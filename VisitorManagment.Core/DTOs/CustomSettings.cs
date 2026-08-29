using Microsoft.Extensions.Configuration;
using System.IO;

namespace ITOWebApiClient.DTOs
{
    public sealed class CustomSettings
    {
        private static CustomSettings _instance = null;
        private static readonly object padlock = new object();
        public readonly string _connectionString = string.Empty;
        public readonly string _authurl = string.Empty;
        public readonly string _smsurl = string.Empty;
        public readonly string _PersonelUrl = string.Empty;
        public readonly string _OrganUrl = string.Empty;
        public readonly string _TashvighatUrl = string.Empty;
        public readonly string _TanbihatUrl = string.Empty;
        public readonly string _EnteghalatUrl = string.Empty;
        public readonly string _TashilateMaskanUrl = string.Empty;
        public readonly string _TashilateDabirKhaneUrl = string.Empty;
        public readonly string _PersonFamilyUrl = string.Empty;
        public readonly string _FajrUrl = string.Empty;
        public readonly string _TashilatDastor = string.Empty;
        public readonly string _TashilatOther = string.Empty;
        public readonly string _Exam = string.Empty;
        public readonly string _Fish = string.Empty;
        public readonly string _Moeeser = string.Empty;

        public readonly string _scope = string.Empty;
        public readonly string _clientsecret = string.Empty;
        public readonly string _clientid = string.Empty;
        public readonly string _username = string.Empty;
        public readonly string _password = string.Empty;
        //*********************************************************
        public readonly string _Cardurl = string.Empty;

        public readonly string _scopeCardIsar = string.Empty;
        public readonly string _clientsecretCardIsar = string.Empty;
        public readonly string _clientidCardIsra = string.Empty;
        public readonly string _usernameCardIsar = string.Empty;
        public readonly string _passwordCardIsar = string.Empty;
        private CustomSettings()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
                .Build();

            _authurl = configuration.GetValue<string>("ApiResourceBaseUrls:AuthServer");
            _smsurl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_SMS");
            _PersonelUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Personel");
            _OrganUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Organ");
            _TashvighatUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Tashvighat");
            _TanbihatUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Tanbihat");
            _EnteghalatUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Enteghalat");
            _TashilateMaskanUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_TashilatMaskan");
            _TashilateDabirKhaneUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_TashilatDabirkhane");
            //_TashilateDabirKhaneUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_TashilatDabirkhane");
            _PersonFamilyUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_PersonFamily");
            _FajrUrl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Fajr");
            _TashilatDastor = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_TashilatDastor");
            _TashilatOther = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_TashilatOther");
            _Exam = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Exam");
            _Fish = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_FishInfo");
            _Moeeser = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Moeeserin");


            _scope = configuration.GetValue<string>("ApiClientInfo:Scope");
            _clientsecret = configuration.GetValue<string>("ApiClientInfo:ClientSecret");
            _clientid = configuration.GetValue<string>("ApiClientInfo:ClientId");
            _username = configuration.GetValue<string>("ApiClientInfo:UserName");
            _password = configuration.GetValue<string>("ApiClientInfo:Password");
            //************************************************************************************

            _Cardurl = configuration.GetValue<string>("ApiResourceBaseUrls:Ito_Card");
            _scopeCardIsar = configuration.GetValue<string>("ApiClientCardInfo:Scope");
            _clientsecretCardIsar = configuration.GetValue<string>("ApiClientCardInfo:ClientSecret");
            _clientidCardIsra = configuration.GetValue<string>("ApiClientCardInfo:ClientId");
            _usernameCardIsar = configuration.GetValue<string>("ApiClientCardInfo:UserName");
            _passwordCardIsar = configuration.GetValue<string>("ApiClientCardInfo:Password");

        }

        public static CustomSettings Instance
        {



            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new CustomSettings();
                    }
                    return _instance;
                }
            }
        }

        public string ConnectionString
        {
            get => _connectionString;
        }

        public string ApiSmsUrl
        {
            get => _smsurl;

        }

        public string AuthenticationSrverUrl
        {
            get => _authurl;

        }

        public string Scope
        {
            get => _scope;

        }

        public string ClientSecret
        {
            get => _clientsecret;

        }

        public string ClientId
        {
            get => _clientid;

        }

        public string ROPC_UserName
        {
            get => _username;

        }

        public string ROPC_Password
        {
            get => _password;

        }

        //********************************************

        public string ApiCardIsarUrl
        {
            get => _Cardurl;

        }

        public string ScopeCardIsar
        {
            get => _scopeCardIsar;

        }

        public string ClientSecretCardIsar
        {
            get => _clientsecretCardIsar;

        }

        public string ClientIdCardIsar
        {
            get => _clientidCardIsra;

        }

        public string ROPC_UserNameCardIsar
        {
            get => _usernameCardIsar;

        }

        public string ROPC_PasswordCardIsar
        {
            get => _passwordCardIsar;

        }

        public string ApiPersonelUrl
        {
            get => _PersonelUrl;

        }

        public string ApiOrganUrl
        {
            get => _OrganUrl;

        }
        public string ApiTashvighatUrl
        {
            get => _TashvighatUrl;

        }

        public string ApiTanbihat
        {
            get => _TanbihatUrl;

        }
        public string ApiEnteghalat
        {
            get => _EnteghalatUrl;

        }

        public string ApiTashilatMaskan
        {
            get => _TashilateMaskanUrl;

        }

        public string ApiTashilatDabirKhaneh
        {
            get => _TashilateDabirKhaneUrl;

        }

        public string ApiPersonFamily
        {
            get => _PersonFamilyUrl;

        }

        public string ApiFajr
        {
            get => _FajrUrl;

        }

        public string ApiTashilatDastor
        {
            get => _TashilatDastor;

        }

        public string ApiTashilatOther
        {
            get => _TashilatOther;

        }

        public string ApiExam
        {
            get => _Exam;

        }

        public string ApiFish
        {
            get => _Fish;

        }

        public string ApiMoeeser
        {
            get => _Moeeser;

        }

    }
}


