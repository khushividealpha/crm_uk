//using CRMUKMTPApi.Models;
//using MediatR;
//using MetaQuotes.MT5CommonAPI;
//using MT5LIB.Helpers;

//namespace CRMUKMTPApi.CommandHandler
//{
//    public class CreateClientCommand :IRequest<CreateClientResponse>
//    {
//        public CreateClientRequest Params { get; }
//        public CreateClientCommand(CreateClientRequest parameters)
//        {
//            Params = parameters;
//        }
//    }
//    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, CreateClientResponse>
//    {
//        public Task<CreateClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                if (Utilities.Manager == null)
//                {
//                    return Task.FromResult(new CreateClientResponse
//                    {
//                        status = false,
//                        reason = "MT5 Manager not initialized"
//                    });
//                }

//                var manager = Utilities.Manager;

//                var client = manager.ClientCreate();
//                if (client == null)
//                {
//                    return Task.FromResult(new CreateClientResponse
//                    {
//                        status = false,
//                        reason = "Failed to create client object"
//                    });
//                }

//                if (!TrySetClientType(client, request.Params.ClientType))
//                {
//                    client.Release();
//                    client.ClientType(CIMTClient.EnClientType.CLIENT_TYPE_UNDEFINED);

//                }

//                if (!TrySetKycStatus(client, request.Params.KycStatus))
//                {
//                    client.Release();
//                    client.KYCStatus(CIMTClient.EnKYCStatus.KYC_STATUS_UNDEFINED);
//                }
//                //client.AddressCity(request.Params.PersonCity);
//                //client.AddressCountry(request.Params.PersonAddress);
//                //client.AddressState(request.Params.PersonState);
//                //client.ClientExternalID(request.Params.ClientExternalID);
//                //client.CompanyName(request.Params.CompanyName);
//                client.PersonAnnualDeposit(request.Params.AnnaulDeposit);
//                client.PersonAnnualIncome(request.Params.AnnualIncome);
//                client.PersonNetWorth(request.Params.NetWorth);

//                if (!TrySetClientStatus(client, request.Params.ClientStatus))
//                {
//                    client.Release();
//                    client.ClientStatus(CIMTClient.EnClientStatus.CLIENT_STATUS_UNREGISTERED);
//                }
//                if (!TrySetEmploymentStatus(client, request.Params.EmploymentStatus))
//                {
//                    client.Release();
//                    client.PersonEmployment(CIMTClient.EnEmployment.EMPLOYMENT_UNEMPLOYED);
//                }
//                if (!TrySetEmploymentIndustry(client, request.Params.EmploymentIndus))
//                {
//                    client.Release();
//                    client.PersonIndustry(CIMTClient.EnEmploymentIndustry.INDUSTRY_NONE);
//                }
//                if (!TrySetEducationLevel(client, request.Params.PersonEdu))
//                {
//                    client.Release();
//                    client.PersonEducation(CIMTClient.EnEducationLevel.EDUCATION_LEVEL_NONE);
//                }
//                if (!TrySetGender(client, request.Params.Gender))
//                {
//                    client.Release();
//                    client.PersonGender(CIMTClient.EnGender.GENDER_UNSPECIFIED);
//                }
//                if (!TrySetClientOrgin(client, request.Params.ClientOrigin))
//                {
//                    client.Release();
//                    client.ClientOrigin(CIMTClient.EnClientOrigin.CLIENT_ORIGIN_MANUAL);
//                }
//                client.ExperienceFX(CIMTClient.EnTradingExperience.EXPERIENCE_LESS_1_YEAR);
//                client.ExperienceFutures(CIMTClient.EnTradingExperience.EXPERIENCE_1_3_YEAR);
//                client.ExperienceStocks(CIMTClient.EnTradingExperience.EXPERIENCE_ABOVE_3_YEAR);
//                client.ExperienceCFD(CIMTClient.EnTradingExperience.EXPERIENCE_FIRST);
//                client.DateCreated(DateTime.UtcNow.ToFileTimeUtc());
//                client.ClientOrigin(CIMTClient.EnClientOrigin.CLIENT_ORIGIN_MANUAL);
//                client.ContactEmail(request.Params.PersonEmail);
//                client.ContactPhone(request.Params.PersonPhone);
//              client.PersonGender(CIMTClient.EnGender.GENDER_LAST);

//                client.PersonName(request.Params.PersonName);
//                client.PersonLastName(request.Params.PersonLastName);
//                client.PersonMiddleName(request.Params.PersonMiddleName);
//               client.AssignedManager();


//                client.PersonDocumentExtra(request.Params.PersonDocumentExtra);

//                if (request.Params.PersonBirthDate.HasValue)
//                    client.PersonBirthDate(
//                        request.Params.PersonBirthDate.Value.ToFileTimeUtc());

//                if (request.Params.PersonDocumentDate.HasValue)
//                    client.PersonDocumentDate(
//                        request.Params.PersonDocumentDate.Value.ToFileTimeUtc());

//                if (request.Params.PersonDocumentExpiration.HasValue)
//                    client.PersonDocumentExpiration(
//                        request.Params.PersonDocumentExpiration.Value.ToFileTimeUtc());

//                client.PersonAnnualIncome(request.Params.PersonAnnualIncome);

//                var retAdd = manager.ClientAdd(client);

//                if (retAdd != MTRetCode.MT_RET_OK)
//                {
//                    client.Release();

//                    return Task.FromResult(new CreateClientResponse
//                    {
//                        status = false,
//                        reason = GetClientAddError(retAdd)
//                    });
//                }

//                var clientId = client.ClientExternalID();
//                client.Release();

//                return Task.FromResult(new CreateClientResponse
//                {
//                    status = true,
//                    clientId = clientId,
//                });
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Error in CreateClientCommandHandler: " + ex.Message, ex);
//            }
//        }
//        private static bool TrySetClientStatus(
//    CIMTClient client,
//    string status)
//        {
//            if (string.IsNullOrWhiteSpace(status))
//                return false;

//            switch (status.ToUpperInvariant())
//            {
//                case "ACTIVE":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_ACTIVE);
//                    return true;
//                case "INACTIVE":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_INACTIVE);
//                    return true;
//                case "UNREGISTERED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_UNREGISTERED);
//                    return true;
//                case "REGISTERED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_REGISTERED);
//                    return true;
//                case "NOTINTERESTED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_NOTINTERESTED);
//                    return true;
//                case "APPLICATIONINCOMPLETED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_APPLICATION_INCOMPLETED);
//                    return true;
//                case "APPLICATIONCOMPLETED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_APPLICATION_COMPLETED);
//                    return true;
//                case "APPLICATIONINFORMATION":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_APPLICATION_INFORMATION);
//                    return true;
//                case "APPLICATIONREJECTED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_APPLICATION_REJECTED);
//                    return true;
//                case "TERMINATED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_TERMINATED);
//                    return true;
//                case "APPROVED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_APPROVED);
//                    return true;
//                case "FIRST":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_FIRST);
//                    return true;
//                case "funded":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_FUNDED);
//                    return true;
//                case "LAST":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_LAST);
//                    return true;
//                case "SUSPENDED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_SUSPENDED);
//                    return true;
//                case "terminated":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_TERMINATED);
//                    return true;

//                case "CLOSED":
//                    client.ClientStatus(
//                        CIMTClient.EnClientStatus.CLIENT_STATUS_CLOSED);
//                    return true;

//                default:
//                    return false;
//            }
//        }
//        private static bool TrySetKycStatus(
//            CIMTClient client,
//            string kycStatus)
//        {
//            if (string.IsNullOrWhiteSpace(kycStatus))
//                return false;

//            switch (kycStatus.ToUpperInvariant())
//            {
//                case "FIRST":
//                    client.KYCStatus(
//                        CIMTClient.EnKYCStatus.KYC_STATUS_FIRST);
//                    return true;

//                case "LAST":
//                    client.KYCStatus(
//                        CIMTClient.EnKYCStatus.KYC_STATUS_LAST);
//                    return true;

//                case "DECLINED":
//                    client.KYCStatus(
//                        CIMTClient.EnKYCStatus.KYC_STATUS_DECLINED);
//                    return true;

//                case "UNDEFINED":
//                    client.KYCStatus(
//                        CIMTClient.EnKYCStatus.KYC_STATUS_UNDEFINED);
//                    return true;
//                case "APPROVED":
//                    client.KYCStatus(
//                        CIMTClient.EnKYCStatus.KYC_STATUS_APPROVED);
//                    return true;


//                default:
//                    return false;
//            }
//        }

//        private static bool TrySetEmploymentStatus(
//         CIMTClient client,
//         string employmentStatus)
//        {
//            if (string.IsNullOrWhiteSpace(employmentStatus))
//                return false;

//            switch (employmentStatus.ToUpperInvariant())
//            {
//                case "UNEMPLOYMED":
//                    client.PersonEmployment(
//                       CIMTClient.EnEmployment.EMPLOYMENT_UNEMPLOYED);
//                    return true;
//                case "EMPLOYED":
//                    client.PersonEmployment(
//                       CIMTClient.EnEmployment.EMPLOYMENT_EMPLOYED);
//                    return true;
//                case "SELFEMPLOYED":
//                    client.PersonEmployment(
//                       CIMTClient.EnEmployment.EMPLOYMENT_SELF_EMPLOYED);
//                    return true;

//                default:
//                    return false;
//            }
//        }
//        private static bool TrySetEmploymentIndustry(
//       CIMTClient client,
//       string employmentIndus)
//        {
//            if (string.IsNullOrWhiteSpace(employmentIndus))
//                return false;

//            switch (employmentIndus.ToUpperInvariant())
//            {
//                case "None":
//                    client.PersonIndustry(
//                       CIMTClient.EnEmploymentIndustry.INDUSTRY_NONE);
//                    return true;
//                case "Agriculture":
//                    client.PersonIndustry(
//                       CIMTClient.EnEmploymentIndustry.INDUSTRY_AGRICULTURE);
//                    return true;
//                case "Engineering":
//                    client.PersonIndustry(
//                       CIMTClient.EnEmploymentIndustry.INDUSTRY_ENGINEERING);
//                    return true;
//                default:
//                    return false;
//            }
//        }
//        private static bool TrySetEducationLevel(
//   CIMTClient client,
//   string eduLevel)
//        {
//            if (string.IsNullOrWhiteSpace(eduLevel))
//                return false;

//            switch (eduLevel.ToUpperInvariant())
//            {
//                case "None":
//                    client.PersonEducation(
//                       CIMTClient.EnEducationLevel.EDUCATION_LEVEL_NONE);
//                    return true;
//                case "Bachelor":
//                    client.PersonEducation(
//                      CIMTClient.EnEducationLevel.EDUCATION_LEVEL_BACHELOR);
//                    return true;
//                case "HighSchool":
//                    client.PersonEducation(
//                       CIMTClient.EnEducationLevel.EDUCATION_LEVEL_HIGH_SCHOOL);
//                    return true;
//                default:
//                    return false;
//            }
//        }

//        private static bool TrySetGender(
//CIMTClient client,
//string gender)
//        {
//            if (string.IsNullOrWhiteSpace(gender))
//                return false;

//            switch (gender.ToUpperInvariant())
//            {
//                case "MALE":
//                    client.PersonGender(
//                       CIMTClient.EnGender.GENDER_MALE);
//                    return true;
//                case "FEMALE":
//                    client.PersonGender(
//                       CIMTClient.EnGender.GENDER_FEMALE);
//                    return true;
//                case "UNSPECIFIED":
//                    client.PersonGender(
//                       CIMTClient.EnGender.GENDER_UNSPECIFIED);
//                    return true;
//                default:
//                    return false;
//            }
//        }




//        private static bool TrySetClientOrgin(
//CIMTClient client,
//string clientorigin)
//        {
//            if (string.IsNullOrWhiteSpace(clientorigin))
//                return false;

//            switch (clientorigin.ToUpperInvariant())
//            {
//                case "MANUAL":
//                    client.ClientOrigin(
//                       CIMTClient.EnClientOrigin.CLIENT_ORIGIN_MANUAL);
//                    return true;
//                case "DEMO":
//                    client.ClientOrigin(
//                       CIMTClient.EnClientOrigin.CLIENT_ORIGIN_DEMO);
//                    return true;
//                case "REAL":
//                    client.ClientOrigin(
//                      CIMTClient.EnClientOrigin.CLIENT_ORIGIN_REAL);
//                    return true;
//                default:
//                    return false;
//            }
//        }

//        private static bool TrySetClientType(CIMTClient client, string clientType)

//        {
//            if (string.IsNullOrWhiteSpace(clientType))
//                return false;

//            switch (clientType.ToUpperInvariant())
//            {
//                case "UNDEFINED":
//                    client.ClientType(CIMTClient.EnClientType.CLIENT_TYPE_UNDEFINED);
//                    return true;

//                case "INDIVIDUAL":
//                    client.ClientType(CIMTClient.EnClientType.CLIENT_TYPE_INDIVIDUAL);
//                    return true;

//                case "CORPORATE":
//                    client.ClientType(CIMTClient.EnClientType.CLIENT_TYPE_CORPORATE);
//                    return true;

//                case "FUND":
//                    client.ClientType(CIMTClient.EnClientType.CLIENT_TYPE_FUND);
//                    return true;

//                default:
//                    return false;
//            }
//        }

//        private static string GetClientAddError(MTRetCode code)
//        {
//            return code switch
//            {
//                MTRetCode.MT_RET_ERR_PARAMS =>
//                    "Invalid client parameters.",



//                MTRetCode.MT_RET_ERR_NETWORK =>
//                    "MT5 network error.",

//                MTRetCode.MT_RET_ERR_TIMEOUT =>
//                    "MT5 request timeout.",

//                _ =>
//                    "MT5 client creation failed."
//            };
//        }

//    }
//}


using CRMUKMTPApi.Models;
using MediatR;
using MetaQuotes.MT5CommonAPI;
using MT5LIB.Helpers;

public class CreateClientCommand : IRequest<CreateClientResponse>
{
    public CreateClientRequest Params { get; }
    public CreateClientCommand(CreateClientRequest parameters)
    {
        Params = parameters;
    }
}
public class CreateClientCommandHandler
    : IRequestHandler<CreateClientCommand, CreateClientResponse>
{
    public Task<CreateClientResponse> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken)
    {
        if (Utilities.Manager == null)
        {
            return Task.FromResult(Fail("MT5 Manager not initialized"));
        }

        var manager = Utilities.Manager;
        var client = manager.ClientCreate();
        if (client == null)
            return Task.FromResult(Fail("Failed to create MT5 client object"));

        try
        {
            /* ================= REQUIRED FIELDS ================= */

            //if (string.IsNullOrWhiteSpace(request.Params.Group))
            //    return FailAndRelease(client, "Client group is required");

            //client.Group(request.Params.Group);

            if (string.IsNullOrWhiteSpace(request.Params.PersonName))
                return FailAndRelease(client, "First name is required");




            client.PersonName(request.Params.PersonName );
            client.PersonLastName(request.Params.PersonLastName ?? "");
          


            /* ================= OPTIONAL SAFE FIELDS ================= */

            client.PersonMiddleName(request.Params.PersonMiddleName ?? "");
            client.ContactEmail(request.Params.PersonEmail ?? "");
            client.ContactPhone(request.Params.PersonPhone ?? "");
            client.ContactPhone(request.Params.PersonPhone ?? "");
            client.ClientStatus(ParseClientStatus(request.Params.ClientStatus));
            client.ClientType(ParseClientType(request.Params.ClientType));
            client.ClientOrigin(ParseClientOrigin(request.Params.ClientOrigin));
            client.KYCStatus(ParseKycStatus(request.Params.KycStatus));
            client.PersonGender(ParseGender(request.Params.Gender));
            client.PersonEmployment(ParseEmployment(request.Params.EmploymentStatus));
            client.PersonIndustry(ParseIndustry(request.Params.EmploymentIndus));
            client.PersonEducation(ParseEducation(request.Params.PersonEdu));
            client.ContactPreferred(CIMTClient.EnPreferredCommunication.PREFERRED_COMMUNICATION_UNDEFINED);
            client.PersonAnnualIncome(request.Params.PersonAnnualIncome);
            client.PersonAnnualDeposit(request.Params.AnnaulDeposit);
            client.PersonNetWorth(request.Params.NetWorth);
            client.AddressCountry(request.Params.PersonCountry ?? "");
            client.AddressCity(request.Params.PersonCity ?? "");
            client.AddressState(request.Params.PersonState ?? "");
            client.CompanyName(request.Params.CompanyName ?? "");

            client.AddressStreet(request.Params.PersonAddress ?? "");

            if (request.Params.PersonBirthDate.HasValue)
                client.PersonBirthDate(
                    request.Params.PersonBirthDate.Value.ToFileTimeUtc());

            if (request.Params.PersonDocumentDate.HasValue)
                client.PersonDocumentDate(
                    request.Params.PersonDocumentDate.Value.ToFileTimeUtc());

            if (request.Params.PersonDocumentExpiration.HasValue)
                client.PersonDocumentExpiration(
                    request.Params.PersonDocumentExpiration.Value.ToFileTimeUtc());

            client.DateCreated(DateTime.UtcNow.ToFileTimeUtc());
            client.ExperienceFX(CIMTClient.EnTradingExperience.EXPERIENCE_LESS_1_YEAR);
            client.ExperienceFutures(CIMTClient.EnTradingExperience.EXPERIENCE_1_3_YEAR);
            client.ExperienceStocks(CIMTClient.EnTradingExperience.EXPERIENCE_ABOVE_3_YEAR);
            client.ExperienceCFD(CIMTClient.EnTradingExperience.EXPERIENCE_FIRST);

            /* ================= CREATE CLIENT ================= */

            var result = manager.ClientAdd(client);

            if (result != MTRetCode.MT_RET_OK)
                return FailAndRelease(client, GetClientAddError(result));

            var externalId = client.ClientExternalID();
            client.Release();

            return Task.FromResult(new CreateClientResponse
            {
                status = true,
                clientId = externalId
            });
        }
        catch (Exception ex)
        {
            client.Release();
            throw new Exception("CreateClient failed", ex);
        }
    }

    /* ================= HELPERS ================= */

    private static CreateClientResponse Fail(string reason) =>
        new() { status = false, reason = reason };

    private static Task<CreateClientResponse> FailAndRelease(
        CIMTClient client, string reason)
    {
        client.Release();
        return Task.FromResult(Fail(reason));
    }


    private static CIMTClient.EnClientStatus ParseClientStatus(string? v) =>
    v?.ToUpperInvariant() switch
    {
        "ACTIVE" => CIMTClient.EnClientStatus.CLIENT_STATUS_ACTIVE,
        "INACTIVE" => CIMTClient.EnClientStatus.CLIENT_STATUS_INACTIVE,
        "REGISTERED" => CIMTClient.EnClientStatus.CLIENT_STATUS_REGISTERED,
        "APPROVED" => CIMTClient.EnClientStatus.CLIENT_STATUS_APPROVED,
        _ => CIMTClient.EnClientStatus.CLIENT_STATUS_UNREGISTERED
    };

    private static CIMTClient.EnClientType ParseClientType(string? v) =>
        v?.ToUpperInvariant() switch
        {
            "INDIVIDUAL" => CIMTClient.EnClientType.CLIENT_TYPE_INDIVIDUAL,
            "CORPORATE" => CIMTClient.EnClientType.CLIENT_TYPE_CORPORATE,
            _ => CIMTClient.EnClientType.CLIENT_TYPE_UNDEFINED
        };

    private static CIMTClient.EnClientOrigin ParseClientOrigin(string? v) =>
        v?.ToUpperInvariant() switch
        {
            "DEMO" => CIMTClient.EnClientOrigin.CLIENT_ORIGIN_DEMO,
            "REAL" => CIMTClient.EnClientOrigin.CLIENT_ORIGIN_REAL,
            _ => CIMTClient.EnClientOrigin.CLIENT_ORIGIN_MANUAL
        };

    private static CIMTClient.EnKYCStatus ParseKycStatus(string? v) =>
        v?.ToUpperInvariant() switch
        {
            "APPROVED" => CIMTClient.EnKYCStatus.KYC_STATUS_APPROVED,
            "DECLINED" => CIMTClient.EnKYCStatus.KYC_STATUS_DECLINED,
            _ => CIMTClient.EnKYCStatus.KYC_STATUS_UNDEFINED
        };

    private static CIMTClient.EnGender ParseGender(string? v) =>
        v?.ToUpperInvariant() switch
        {
            "MALE" => CIMTClient.EnGender.GENDER_MALE,
            "FEMALE" => CIMTClient.EnGender.GENDER_FEMALE,
            _ => CIMTClient.EnGender.GENDER_UNSPECIFIED
        };

    private static CIMTClient.EnEmployment ParseEmployment(string? v) =>
        v?.ToUpperInvariant() switch
        {
            "EMPLOYED" => CIMTClient.EnEmployment.EMPLOYMENT_EMPLOYED,
            "SELFEMPLOYED" => CIMTClient.EnEmployment.EMPLOYMENT_SELF_EMPLOYED,
            _ => CIMTClient.EnEmployment.EMPLOYMENT_UNEMPLOYED
        };

    private static CIMTClient.EnEmploymentIndustry ParseIndustry(string? v) =>
        v?.ToUpperInvariant() switch
        {
            "AGRICULTURE" => CIMTClient.EnEmploymentIndustry.INDUSTRY_AGRICULTURE,
            "ENGINEERING" => CIMTClient.EnEmploymentIndustry.INDUSTRY_ENGINEERING,
            _ => CIMTClient.EnEmploymentIndustry.INDUSTRY_NONE
        };

    private static CIMTClient.EnEducationLevel ParseEducation(string? v) =>
        v?.ToUpperInvariant() switch
        {
            "BACHELOR" => CIMTClient.EnEducationLevel.EDUCATION_LEVEL_BACHELOR,
            "HIGHSCHOOL" => CIMTClient.EnEducationLevel.EDUCATION_LEVEL_HIGH_SCHOOL,
            _ => CIMTClient.EnEducationLevel.EDUCATION_LEVEL_NONE
        };

    private static string GetClientAddError(MTRetCode code)
    {
        return code switch
        {
            MTRetCode.MT_RET_ERR_PARAMS =>
                "Invalid client parameters.",



            MTRetCode.MT_RET_ERR_NETWORK =>
                "MT5 network error.",

            MTRetCode.MT_RET_ERR_TIMEOUT =>
                "MT5 request timeout.",

            _ =>
                "MT5 client creation failed."
        };
    }
}