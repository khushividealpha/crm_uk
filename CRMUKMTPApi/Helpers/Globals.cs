
using Apmcrm.V1.Msgs;
using Apmcrm.V1.Msgs.Types;
using CRMUKMTPApi.Models;
using Google.Protobuf;
using ProtoBuf;
using Serilog;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Globalization;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Reflection;

namespace CRMUKMTPApi.Helpers;

public class Globals
{
    public static ConcurrentDictionary<ulong, ConcurrentBag<WebSocket>> ConnectedSockets = new();
    public static T ConvertNumric<T>(object val)
    {
        T outVal = (T)Convert.ChangeType(0, typeof(T));

        try
        {
            if (!string.IsNullOrEmpty(val.ToString()))
            {
                try
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                catch (Exception ex)
                {

                    return default(T);
                }
            }
            else
            {
                return outVal;
            }
        }
        catch
        {

            //LogWriter.LogWriter.WriteToLog(val==null?"Null error":val.ToString());
            throw;
        }
    }
    public static Expression<Func<T, bool>>? FilterByExpression<T>(
    ParamModel param,
   Microsoft.Extensions.Logging.ILogger logger)
    {
        try
        {
            string propertyName = param.Filter;
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, propertyName);
            var propertyType = property.Type;

            Expression body;

            if (propertyType == typeof(double) || propertyType == typeof(float) || propertyType == typeof(decimal))
            {
                if (param.Filtervalue.Trim() == "-")
                {
                    // Special case: filter for < 0
                    var zero = Expression.Constant(Convert.ChangeType(0, propertyType), propertyType);
                    body = Expression.LessThan(property, zero);
                }
                else
                {
                    // Usual equality filter
                    object convertedValue;
                    try
                    {
                        convertedValue = Convert.ChangeType(param.Filtervalue, propertyType);
                    }
                    catch
                    {
                        logger.LogError($"Filter value '{param.Filtervalue}' could not be converted to type {propertyType} for filtering on {propertyName}");
                        return null;
                    }

                    var constant = Expression.Constant(convertedValue, propertyType);
                    body = Expression.Equal(property, constant);
                }
            }
            else if (propertyType == typeof(string))
            {
                var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
                var propertyToLower = Expression.Call(property, toLowerMethod);
                var constant = Expression.Constant(param.Filtervalue.ToLower());
                body = Expression.Equal(propertyToLower, constant);
            }
            else
            {
                object convertedValue;
                try
                {
                    convertedValue = Convert.ChangeType(param.Filtervalue, propertyType);
                }
                catch
                {
                    logger.LogError($"Filter value '{param.Filtervalue}' could not be converted to type {propertyType} for filtering on {propertyName}");
                    return null;
                }

                var constant = Expression.Constant(convertedValue, propertyType);
                body = Expression.Equal(property, constant);
            }

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error building dynamic filter expression on {param.Filter} with value {param.Filtervalue}");
            return null;
        }
    }

    //public static Func<IQueryable<T>, IOrderedQueryable<T>>? SortByExpression<T>(
    //ParamModel param,
    //Microsoft.Extensions.Logging.ILogger logger)
    //{
    //    try
    //    {
    //        if (string.IsNullOrWhiteSpace(param.Sort))
    //            return null;

    //        var sortFields = param.Sort.Split(',', StringSplitOptions.RemoveEmptyEntries)
    //                                    .Select(s => s.Trim())
    //                                    .ToList();

    //        Func<IQueryable<T>, IOrderedQueryable<T>> orderingFunc = query =>
    //        {
    //            IOrderedQueryable<T>? orderedQuery = null;

    //            for (int i = 0; i < sortFields.Count; i++)
    //            {

    //                string sortField = sortFields[i];
    //                bool ascending = !sortField.StartsWith("-");
    //                string propertyName = ascending ? sortField : sortField.Substring(1);


    //                var parameter = Expression.Parameter(typeof(T), "x");
    //                var property = Expression.PropertyOrField(parameter, propertyName);
    //                var lambda = Expression.Lambda(property, parameter);

    //                string methodName;
    //                if (i == 0)
    //                {
    //                    methodName = ascending ? "OrderBy" : "OrderByDescending";
    //                }
    //                else
    //                {
    //                    methodName = ascending ? "ThenBy" : "ThenByDescending";
    //                }

    //                var method = typeof(Queryable).GetMethods()
    //                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
    //                    .MakeGenericMethod(typeof(T), property.Type);

    //                if (i == 0)
    //                {
    //                    orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
    //                }
    //                else
    //                {
    //                    orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { orderedQuery!, lambda })!;
    //                }
    //            }

    //            return orderedQuery!;
    //        };

    //        return orderingFunc;
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, $"Error building dynamic sort expression on {param.Sort}");
    //        return null;
    //    }
    //}

    public static Func<IQueryable<T>, IOrderedQueryable<T>>? SortByExpression<T>(
    ParamModel param,
    Microsoft.Extensions.Logging.ILogger logger)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(param.Sort))
                return null;
            List<string> filterField = new List<string>();
            var sortFields = param.Sort.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim())
                                        .ToList();

            // First validate all properties exist before building the expression
            var type = typeof(T);
            foreach (var sortField in sortFields)
            {
                string propertyName = sortField.StartsWith("-") ? sortField.Substring(1) : sortField;

                // Check if property exists (case-sensitive)
                var property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    logger.LogWarning($"Property '{propertyName}' not found on type {type.Name}");
                }
                else
                {
                    filterField.Add(sortField);
                }
            }
            if (filterField.Count == 0) return null;
            Func<IQueryable<T>, IOrderedQueryable<T>> orderingFunc = query =>
            {
                IOrderedQueryable<T>? orderedQuery = null;

                for (int i = 0; i < filterField.Count; i++)
                {
                    string sortField = filterField[i];
                    bool ascending = !sortField.StartsWith("-");
                    string propertyName = ascending ? sortField : sortField.Substring(1);

                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.PropertyOrField(parameter, propertyName);
                    var lambda = Expression.Lambda(property, parameter);

                    string methodName = i == 0
                        ? (ascending ? "OrderBy" : "OrderByDescending")
                        : (ascending ? "ThenBy" : "ThenByDescending");

                    var method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(T), property.Type);

                    orderedQuery = i == 0
                        ? (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, lambda })!
                        : (IOrderedQueryable<T>)method.Invoke(null, new object[] { orderedQuery!, lambda })!;
                }

                return orderedQuery!;
            };

            return orderingFunc;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error building dynamic sort expression on {param.Sort}");
            return null;
        }
    }
    internal static bool ConvertDate(string dateString, out DateTime? date)
    {
        if (DateTime.TryParseExact(dateString, "yyyy-MM-dd|HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dat))
        {
            date = dat;
            return true;
        }
        date = null;
        return false;
    }

    internal static List<ulong> GetLoginId(string loginId)
    {
        if (string.IsNullOrEmpty(loginId)) return new List<ulong>();
        if (!loginId.Contains(",")) return new List<ulong> { ConvertNumric<ulong>(loginId) };
        return loginId.Split(',').Select(x => ConvertNumric<ulong>(x)).ToList();
    }
    internal static List<string> GetSymbols(string symbol)
    {
        if (string.IsNullOrEmpty(symbol)) return new List<string>();
        if (!symbol.Contains(",")) return new List<string> { symbol };
        return symbol.Split(',').Select(x => x).ToList();
    }
    public static T GetColumnValue<T>(DbDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal))
            return default!;

        object value = reader.GetValue(ordinal);
        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        try
        {
            // Explicit handling for unsigned conversions
            if (targetType == typeof(ulong))
            {
                double doubleValue = Convert.ToDouble(value);
                if (doubleValue < 0)
                    throw new InvalidCastException($"Cannot convert negative value '{doubleValue}' to UInt64 for column '{columnName}'.");
                return (T)(object)Convert.ToUInt64(doubleValue);
            }

            if (targetType == typeof(uint))
            {
                double doubleValue = Convert.ToDouble(value);
                if (doubleValue < 0)
                    throw new InvalidCastException($"Cannot convert negative value '{doubleValue}' to UInt32 for column '{columnName}'.");
                return (T)(object)Convert.ToUInt32(doubleValue);
            }

            // General case
            return (T)Convert.ChangeType(value, targetType);
        }
        catch (Exception ex)
        {
            throw new InvalidCastException(
                $"Failed to convert column '{columnName}' value '{value}' of type '{value.GetType()}' to target type '{targetType}'",
                ex
            );
        }
    }

    internal static async Task BroadcastData<T>(ulong loginId, MessageState messageState, MessageType type, ByteString data)
    {
        if (Globals.ConnectedSockets.Any())
        {

            ApmCrmMessage message = new ApmCrmMessage
            {
                Type = type,
                State = messageState,
                Value = data,
            };
            if (Globals.ConnectedSockets.TryGetValue(0, out var sockets))
            {
                foreach (var socket in sockets)
                {
                    if (socket.State == System.Net.WebSockets.WebSocketState.Open)
                    {
                        await socket.SendAsync(new ArraySegment<byte>(message.ToByteArray()), WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                }
            }
            if (Globals.ConnectedSockets.TryGetValue(loginId, out var clientSockets))
            {
                foreach (var socket in clientSockets)
                {
                    if (socket.State == System.Net.WebSockets.WebSocketState.Open)
                    {
                        await socket.SendAsync(new ArraySegment<byte>(message.ToByteArray()), WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                }
            }
        }
    }

    internal static ByteString ConvertToByteString<T>(T data)
    {
        byte[] bytes;
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, data);
            bytes = ms.ToArray();
        }
        // Convert to ByteString
       return ByteString.CopyFrom(bytes);
    }
}
