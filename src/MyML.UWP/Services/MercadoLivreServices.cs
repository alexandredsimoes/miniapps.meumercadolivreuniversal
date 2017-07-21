using MyML.UWP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MyML.UWP.Models.Mercadolivre;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using Windows.ApplicationModel.Resources;
using MyML.UWP.AppStorage;
using Windows.Globalization.NumberFormatting;
using Windows.Storage;
using Windows.UI.Popups;
using MyML.UWP.Services.SettingsServices;
using static System.String;

namespace MyML.UWP.Services
{
    public class MercadoLivreServices : IMercadoLivreService
    {
        private readonly HttpClient _httpClient;
        private readonly IDataService _dataService;
        private readonly SettingsService _settings = SettingsService.Instance;
        private string _userId;
        private string _accessToken;
        private readonly ResourceLoader _resourceLoader;
        public MercadoLivreServices(HttpClient httpClient, IDataService dataService, ResourceLoader resourceLoader)
        {
            _httpClient = httpClient;
            _dataService = dataService;
            _resourceLoader = resourceLoader;

            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paisId">Será ignorado (definido automaticamente)</param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<MLCategorySearchResult>> ListCategories(string paisId)
        {
            var result = await BaseServices<IReadOnlyCollection<MLCategorySearchResult>>
                .GetAsync(Format(Consts.ML_URL_CATEGORIAS, _settings.SelectedCountry)).ConfigureAwait(false);

            Func<string, string> obterIcone = (categories) =>
            {
                var resultado = Empty;
                switch (categories)
                {
                    case "Acessórios para Veículos":
                    case "Accesorios para Vehículos":
                        resultado = "M11.199982,22.900002L20.5,22.900002 20.5,25.599998 11.199982,25.599998z M23.199982,12.199997L26.5,12.199997 26.5,13.5 23.199982,13.5z M5.1999817,12.199997L8.5,12.199997 8.5,13.5 5.1999817,13.5z M14.099976,10.200001L17.099976,15.099998C17.299988,15.299995 17.399994,15.699997 17.399994,16 17.399994,16.799995 16.799988,17.400002 16,17.400002 15.199982,17.400002 14.599976,16.799995 14.599976,16L14.599976,15.900002z M23.399994,6L24.299988,6.7999992 22,9.5 21,8.7000008z M8.8999939,5.7000008L11.199982,8.5 10.199982,9.3999977 7.8999939,6.5999985z M15.199982,3.5L16.5,3.5 16.5,7.5 15.199982,7.5z M16,2.3999977C8.5,2.3999977 2.3999939,8.5 2.3999939,16 2.3999939,23.5 8.5,29.599998 16,29.599998 23.5,29.599998 29.599976,23.5 29.599976,16 29.599976,8.5 23.5,2.3999977 16,2.3999977z M16,0C24.799988,0 32,7.2000008 32,16 32,24.799995 24.799988,32 16,32 7.1999817,32 0,24.799995 0,16 0,7.2000008 7.1999817,0 16,0z";
                        break;
                    case "Brinquedos e Hobbies":
                    case "Juegos y Juguetes":
                    case "Jogos e Brinquedos":
                        resultado = "M30.299999,0L29.700008,2.7000046C29.700008,2.8000031,29.700008,2.9000015,29.600002,3.0999985L29.600002,3.2000046C29.500012,3.4000015,29.299999,3.7000046,29.000012,4L21.700007,11.800003 25.900005,29 23.1,32 16.1,17.599998 10.6,23.5 11.1,28 9.0000011,30.300003 6.5000006,25.099998 1.7000046,22.400002 3.9000021,20.099998 8.2000056,20.700005 13.700006,14.800003 0,7.2000046 2.7000047,4.3000031 19.1,8.9000015 26.299999,1.2000046C26.600002,0.90000153,26.900005,0.70000458,27.200008,0.59999847L27.299999,0.59999847 27.600002,0.59999847z";
                        break;
                    case "Calçados, Roupas e Bolsas":
                    case "Ropa, Calzados y Accesorios":
                    case "Roupas e Acessórios":
                        resultado = "M16,8.6999893L15.099976,11.600016 12.099976,11.600016 14.5,13.400005 13.599976,16.300001 16,14.500012 18.400024,16.300001 17.5,13.400005 19.900024,11.600016 16.900024,11.600016z M2,5.199986L6.2999878,9.6999903 3.7000122,11.400003 0,7.4000001z M30,5.1000104L32,7.299994 28.200012,11.299997 25.599976,9.6000137z M10.299988,0C11.900024,1.3656972E-08,12.900024,0.69998227,12.900024,0.69998215L12.900024,1.100007C12.900024,2.6999841 14.299988,4.0000033 16,4.0000033 17.700012,4.0000033 19.200012,2.6999841 19.200012,1.100007 19.200012,0.89999466 19.200012,0.69998227 19.099976,0.50000046L19.299988,0.50000046C24.599976,-1.5000012,28.799988,4.0000033,28.799988,4.0000033L24.299988,8.9000015 24.299988,22.700001 7.5999756,22.700001 7.5999756,8.7999954 3.0999756,4.199985C5.7999878,0.79998849,8.4000244,1.3656972E-08,10.299988,0z";
                        break;
                    case "Carros, Motos e Outros":
                    case "Autos, Motos y Otros":
                        resultado = "M25.330021,14.976999C24.980015,14.976999 24.696011,15.268999 24.696011,15.627 24.696011,15.986 24.980015,16.278001 25.330021,16.278001 25.680027,16.278001 25.964031,15.986 25.964031,15.627 25.964031,15.268999 25.680027,14.976999 25.330021,14.976999z M7.2669716,14.976999C6.916976,14.976999 6.6329799,15.268999 6.6329799,15.627 6.6329799,15.986 6.916976,16.278001 7.2669716,16.278001 7.6169667,16.278001 7.9019632,15.986 7.9019632,15.627 7.9019632,15.268999 7.6169667,14.976999 7.2669716,14.976999z M25.330021,13.976997C26.230036,13.976997 26.964048,14.716998 26.964048,15.627 26.964048,16.538001 26.230036,17.278002 25.330021,17.278002 24.430006,17.278002 23.695993,16.538001 23.695993,15.627 23.695993,14.716998 24.430006,13.976997 25.330021,13.976997z M7.2669716,13.976997C8.1679592,13.976997 8.9019499,14.716998 8.9019499,15.627 8.9019499,16.538001 8.1679592,17.278002 7.2669716,17.278002 6.3669834,17.278002 5.6329927,16.538001 5.6329927,15.627 5.6329927,14.716998 6.3669834,13.976997 7.2669716,13.976997z M10.621,13.647006L20.621,13.647006 20.621,14.647006 10.621,14.647006z M5.3310547,10.875021C3.4940186,10.875021,2,12.364035,2,14.194022L2,19.079033C2,19.162041,2.0679932,19.229027,2.1530762,19.229027L4.3070068,19.229027 4.3070068,21.379021C4.3070068,21.462029,4.3759766,21.530022,4.4610596,21.530022L7.9229736,21.530022C8.0080566,21.530022,8.0770264,21.462029,8.0770264,21.379021L8.0770264,19.229027 23.922974,19.229027 23.922974,21.379021C23.922974,21.462029,23.992065,21.530022,24.077026,21.530022L27.537964,21.530022C27.623047,21.530022,27.692017,21.462029,27.692017,21.379021L27.692017,19.229027 29.846069,19.229027C29.93103,19.229027,30,19.162041,30,19.079033L30,14.194022C30,12.364035,28.505005,10.875021,26.667969,10.875021z M9.5380383,2.0000019C7.5190377,2.0000019,5.8750377,3.6730032,5.8750377,5.7300048L5.8750377,8.875021 26.125038,8.875021 26.125038,5.7300048C26.125038,3.6730032,24.481037,2.0000019,22.462038,2.0000019z M9.5380383,0L22.462038,0C25.585037,0,28.125038,2.5700026,28.125038,5.7300048L28.125038,9.0785616 28.251793,9.1145487C30.420927,9.7890069,32,11.810973,32,14.194022L32,19.079033C32,20.26403,31.032959,21.229027,29.846069,21.229027L29.692017,21.229027 29.692017,21.379021C29.692017,22.565025,28.724976,23.530022,27.537964,23.530022L24.077026,23.530022C22.890015,23.530022,21.922974,22.565025,21.922974,21.379021L21.922974,21.229027 10.077026,21.229027 10.077026,21.379021C10.077026,22.565025,9.1099854,23.530022,7.9229736,23.530022L4.4610596,23.530022C3.2730713,23.530022,2.3070068,22.565025,2.3070068,21.379021L2.3070068,21.229027 2.1530762,21.229027C0.96606445,21.229027,0,20.26403,0,19.079033L0,14.194022C0,11.810973,1.5784278,9.7890069,3.7473207,9.1145487L3.8750381,9.0782846 3.8750381,5.7300048C3.8750379,2.5700026,6.4150381,0,9.5380383,0z";
                        break;
                    case "Casa, Móveis e Decoração":
                    case "Hogar, Muebles y Jardín":
                        resultado = "M9,22L15,22 15,23 9,23z M2,20L2,27 22,27 22,20z M9,13L15,13 15,14 9,14z M2,11L2,18 22,18 22,11z M9,4L15,4 15,5 9,5z M2,2L2,9 22,9 22,2z M1,0L23,0C23.552,0,24,0.44799995,24,1L24,28C24,28.552,23.552,29,23,29L20,29 20,32 18,32 18,29 5,29 5,32 3,32 3,29 1,29C0.44799995,29,0,28.552,0,28L0,1C0,0.44799995,0.44799995,0,1,0z";
                        break;
                    case "Celulares e Telefones":
                    case "Celulares y Telefonía":
                    case "Telemóveis e Telefones":
                        resultado = "M8.0000143,28.300049C7.7000261,28.300049 7.3999768,28.600037 7.3999768,28.900024 7.3999768,29.200012 7.7000261,29.5 8.0000143,29.5L12.899986,29.5C13.200036,29.5 13.500024,29.200012 13.500024,28.900024 13.500024,28.600037 13.200036,28.300049 12.899986,28.300049z M10.700031,16.700012C11.200032,17 11.800009,17.300049 12.700035,17.300049 13.200036,17.300049 13.800013,17.200012 14.399989,17L13.500024,20.100037C12.800011,20.400024 12.200034,20.5 11.599997,20.5 10.800007,20.5 10.300006,20.200012 9.800005,19.900024z M8.0999899,15.600037C8.8000031,15.600037,9.300004,15.800049,9.8999806,16.200012L8.8999796,19.5C8.3999787,19.200012 7.8000017,18.900024 7.0000124,18.900024 6.5000115,18.900024 5.8999739,19 5.2000213,19.300049L6.2000232,16C6.8999758,15.700012,7.599989,15.600037,8.0999899,15.600037z M11.899984,12.5C12.399985,12.800049 13.000023,13.100037 13.800013,13.100037 14.300014,13.100037 14.89999,13 15.600003,12.700012L14.700038,16C14.000025,16.300049 13.399987,16.400024 12.800011,16.400024 12.000021,16.400024 11.399983,16.100037 11.00002,15.800049z M9.300004,11.400024C10.099994,11.400024,10.599995,11.700012,11.099996,12L10.20003,15.300049C9.7000294,15 9.0999918,14.700012 8.3000021,14.700012 7.8000017,14.700012 7.2000251,14.800049 6.5000115,15.100037L7.3999768,11.900024C8.0999899,11.5,8.8000031,11.400024,9.300004,11.400024z M3.0999813,3.7000122C2.700017,3.7000122,2.5000043,4,2.5000043,4.3000488L2.5000043,25.200012C2.5000043,25.5,2.7999928,25.800049,3.0999813,25.800049L17.899996,25.800049C18.200045,25.800049,18.500032,25.5,18.500032,25.200012L18.500032,4.3000488C18.500032,4,18.200045,3.7000122,17.899996,3.7000122z M8.0000143,1.2000122C7.7000261,1.2000122 7.3999768,1.5 7.3999768,1.8000488 7.3999768,2.1000366 7.7000261,2.4000244 8.0000143,2.4000244L12.899986,2.4000244C13.200036,2.4000244 13.500024,2.1000366 13.500024,1.8000488 13.500024,1.5 13.200036,1.2000122 12.899986,1.2000122z M3.7000189,0L17.200043,0C19.200047,0,20.900002,1.7000122,20.900002,3.7000122L20.900002,28.300049C20.900002,30.300049,19.200047,32,17.200043,32L3.7000189,32C1.7000152,32,-3.6463462E-08,30.300049,0,28.300049L0,3.7000122C-3.6463462E-08,1.7000122,1.7000152,0,3.7000189,0z";
                        break;
                    case "Eletrônicos, Áudio e Vídeo":
                    case "Electrónica, Audio y Video":
                    case "Electrónica":
                        resultado = "M2.8739996,24.5L7.8739996,24.5 7.8739996,25.5 2.8739996,25.5z M5.4159999,15.332994C3.9460015,15.332994 2.7490025,16.528996 2.7490025,18 2.7490025,19.471004 3.9460015,20.667006 5.4159999,20.667006 6.8869984,20.667006 8.0829973,19.471004 8.0829973,18 8.0829973,16.528996 6.8869984,15.332994 5.4159999,15.332994z M5.4159999,14.332992C7.4379978,14.332992 9.0829962,15.977996 9.0829964,18 9.0829962,20.022004 7.4379978,21.667007 5.4159999,21.667007 3.3950019,21.667007 1.7490034,20.022004 1.7490034,18 1.7490034,15.977996 3.3950019,14.332992 5.4159999,14.332992z M2.8739996,10.5L7.8739996,10.5 7.8739996,11.5 2.8739996,11.5z M10,8.5L22,8.5 22,9.5 11,9.5 11,26.5 28,26.5 28,9.5 27,9.5 27,8.5 29,8.5 29,27.5 10,27.5z M0.5,6.5L22,6.5 22,7.5 1,7.5 1,29.5 31,29.5 31,7.5 27,7.5 27,6.5 31.5,6.5C31.775999,6.5,32,6.724,32,7L32,30C32,30.275999,31.775999,30.5,31.5,30.5L0.5,30.5C0.22399998,30.5,0,30.275999,0,30L0,7C0,6.724,0.22399998,6.5,0.5,6.5z M23.5,4.9999561L25.5,4.9999561 25.5,20.697958 24.5,21.697958 23.5,20.697958 23.5,12.244956z M23.5,0L25.5,0 25.5,2 23.5,2z";
                        break;
                    case "Games":
                    case "Consolas y Videojuegos":
                    case "Videojogos":
                        resultado = "M25.910353,6.8920306C26.509354,6.8920306 27.008348,7.391025 27.008348,7.9900254 27.008348,8.5900329 26.509354,9.0890268 25.910353,9.0890268 25.310345,9.0890268 24.811352,8.5900329 24.811352,7.9900254 24.811352,7.391025 25.310345,6.8920306 25.910353,6.8920306z M23.712349,4.5940207C24.31135,4.5940207 24.811352,5.0940222 24.811352,5.6930226 24.811352,6.2920231 24.31135,6.792024 23.712349,6.792024 23.113348,6.792024 22.614353,6.2920231 22.614353,5.6930226 22.614353,5.0940222 23.113348,4.5940207 23.712349,4.5940207z M7.0323085,4.3950151L7.1322846,4.3950151 8.7303114,4.3950151 8.7303114,6.0930175 10.328308,6.0930175 10.328308,7.6910285 8.7303114,7.6910285 8.7303114,9.1890339 7.1322846,9.1890339 7.1322846,7.6910285 5.4342818,7.6910285 5.4342818,6.0930175 7.0323085,6.0930175z M24.811352,2.9960095C22.713354,2.9960095 20.916351,4.6940116 20.916351,6.8920306 20.916351,8.989036 22.614353,10.78703 24.811352,10.78703 26.908342,10.78703 28.706352,9.0890268 28.706352,6.8920306 28.706352,4.6940116 26.908342,2.9960095 24.811352,2.9960095z M7.8312916,2.9960095C5.734301,2.9960095 3.9362914,4.6940116 3.9362912,6.792024 3.9362914,8.8890299 5.6342945,10.687038 7.8312916,10.687038 9.9293203,10.687038 11.727299,8.989036 11.727299,6.792024 11.727299,4.6940116 10.029296,2.9960095 7.8312916,2.9960095z M8.0313043,0L8.231287,0C9.2303133,-2.0215248E-07 10.229309,0.3000036 11.227298,0.79901297 11.926305,1.1990079 12.526313,1.6980174 13.025307,2.2970179L19.018335,2.2970179C19.51733,1.6980174 20.117338,1.1990079 20.816345,0.79901297 24.112345,-0.89898925 28.107351,0.40000997 29.805353,3.596017 31.503357,6.8920306 33.50037,17.179043 30.304378,18.87706 27.607349,20.275059 22.014347,15.182044 19.318325,11.586043L12.726325,11.586043C10.029296,15.182044 4.4362931,20.275059 1.7392941,18.87706 -1.5567357,17.179043 0.54028573,6.8920306 2.2382884,3.596017 3.4372971,1.2980073 5.6342945,-2.0215248E-07 8.0313043,0z";
                        break;
                    case "Imóveis":
                    case "Inmuebles":
                        resultado = "M16.208,8.438004L27.884,17.579009 27.884,31.293015 18.392,31.293015 18.392,21.676011 13.44,21.676011 13.44,31.293015 4.0419998,31.293015 4.0419998,17.579009z M16.211,0L22.585,5.1080027 22.585,2.2450008 26.773,2.2450008 26.773,8.4610038 32.000001,12.645006 32.000001,16.808008 16.23,4.1510019 0,17.177008 0,13.013006z";
                        break;
                    case "Informática":
                    case "Computación":
                        resultado = "M10.499023,23.008999L10.147949,27.094007 21.849976,27.094007 21.412964,23.008999z M3.8079834,18.806009L27.972046,18.806009C28.406982,18.806009,28.76001,19.159006,28.76001,19.594004L32,29.516006C32,29.951004,31.645996,30.304001,31.210938,30.304001L0.78796387,30.275009C0.35192871,30.275009,0,29.922012,0,29.487015L3.0200195,19.594004C3.0200195,19.159006,3.3730469,18.806009,3.8079834,18.806009z M5.2600098,2.1719981L5.2600098,16.178993 26.739014,16.178993 26.739014,2.1719981z M3.6789551,0L28.319946,0C28.770996,1.2767487E-07,29.136963,0.3659976,29.136963,0.81698655L29.136963,17.753C29.136963,18.204996,28.770996,18.571009,28.319946,18.571009L3.6789551,18.571009C3.2280273,18.571009,2.8619385,18.204996,2.8619385,17.753L2.8619385,0.81698655C2.8619385,0.3659976,3.2280273,1.2767487E-07,3.6789551,0z";
                        break;
                    case "Mais Categorias":
                    case "Otras categorías":
                    case "Outras categorias":
                        resultado = "M20.53299,0L20.583008,11.763 32,11.763 32,20.442017 20.619995,20.442017 20.666992,31.963013 11.97699,32 11.927002,20.442017 0,20.442017 0,11.762024 11.895996,11.762024 11.842987,0.038024902z";
                        break;                    
                    default:
                        break;
                }
                return resultado;
            };
            if (result != null)
            {
                foreach (var item in result)
                {
                    item.PathData = obterIcone(item.name);
                    Debug.WriteLine(item.name);
                }
                return result.Where(c => !IsNullOrWhiteSpace(c.PathData)).ToList();
            }
            return null;
        }

        #region Métodos referente as questões
        public async Task<Models.Mercadolivre.MLQuestionResultSearch> ListQuestions(params KeyValuePair<string, object>[] attributesAndFilters)
        {
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);


            var url = Consts.GetUrl(Consts.ML_QUESTIONS_URL, _userId, _accessToken);
            var result = await BaseServices<MLQuestionResultSearch>
                .GetAsync(url, attributesAndFilters).ConfigureAwait(false);

            return result;
        }

        public async Task<bool> AnswerQuestion(string questionId, string content)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_QUESTIONS_ANSWER_URL, _accessToken);

            //Monta os parametros
            var parametros = new
            {
                question_id = questionId,
                text = content
            };



            _httpClient.DefaultRequestHeaders.Accept.Clear();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



            var json = new StringContent(JsonConvert.SerializeObject(parametros));
            var response = await _httpClient.PostAsync(url, json).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveQuestion(string questionId)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_REMOVE_QUESTION_URL, questionId, _accessToken);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            var response = await _httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<ProductQuestion> ListQuestionsByProduct(string itemId, int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCT_QUESTIONS, itemId);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            return await BaseServices<ProductQuestion>.GetAsync(url, attributesAndFilters).ConfigureAwait(false);
        }

        public async Task<ProductQuestionContent> GetQuestionDetails(string questionId, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);


            var url = Consts.GetUrl(Consts.ML_QUESTIONS_DETAIL_URL, questionId);

            return await BaseServices<ProductQuestionContent>.GetAsync(url, attributesAndFilters).ConfigureAwait(false);
        }

        //public async Task<MLQuestionResultSearch> ListQuestions(KeyValuePair<string, object>[] attributesOrFilters)
        //{
        //    try
        //    {
        //        _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
        //        var url = String.Format(Consts.ML_QUESTIONS_LIST, _accessToken);

        //        if (attributesOrFilters != null && attributesOrFilters.Length > 0)
        //        {
        //            for (int i = 0; i < attributesOrFilters.Length; i++)
        //            {
        //                url = string.Concat(url, url.Contains("?") ? "&" : "?", attributesOrFilters[i].Key, "=", attributesOrFilters[i].Value, i < (attributesAndFilters.Length - 1) ? "," : string.Empty);
        //            }
        //        }

        //        var r = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

        //        if (r.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var s = await r.Content.ReadAsStringAsync();
        //            var lista = JsonConvert.DeserializeObject<MLQuestionResultSearch>(s);
        //            return lista;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AppLogs.WriteError("MercadoLivreServices.ListQuestions()", ex);
        //    }
        //    return null;
        //}
        #endregion Métodos referente as questões

        #region Métodos referente aos usuários
        public async Task<MLUserInfoSearchResult> GetUserInfo(string _userId, params KeyValuePair<string, object>[] attributesOrFilters)
        {
            var url = Consts.GetUrl(Consts.ML_USER_INFO_URL, _userId);
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            url = Concat(url, "?", _accessToken);
            return await BaseServices<MLUserInfoSearchResult>.GetAsync(url, attributesOrFilters).ConfigureAwait(false);
        }

        public async Task<MLUserInfoSearchResult> GetUserProfile()
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.ML_USER_PROFILE, _accessToken);
            return await BaseServices<MLUserInfoSearchResult>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<IList<UserAddress>> GetUserAddress(int _userId)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.ML_USER_ADDRESSES, _userId, _accessToken);
            return await BaseServices<IList<UserAddress>>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<bool> UpdateUserInfo(object customData)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);

            var url = Consts.GetUrl(Consts.ML_CHANGE_USER_INFO_URL, _userId, _accessToken);
            var jsonCustomData = JsonConvert.SerializeObject(customData);
            var response = await _httpClient.PutAsync(url, new StringContent(jsonCustomData)).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.PartialContent)
            {
                return true;
            }
            return false;
        }

        public async Task<MLAccountBalance> GetUserAccountBalance()
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);

            var url = Consts.GetUrl(Consts.ML_USER_ACCOUNT_BALANCE_URL, _userId, _accessToken);
            return await BaseServices<MLAccountBalance>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<IList<MLBookmarkItem>> GetBookmarkItems()
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_USER_BOOKMARKS_URL, _accessToken);
            var items = await BaseServices<IList<MLBookmarkItem>>.GetAsync(url).ConfigureAwait(false);
            if (items != null)
                foreach (var item in items)
                {
                    item.ItemInfo = await GetItemDetails(item.item_id, new KeyValuePair<string, object>("attributes", "id,thumbnail,price,title")).ConfigureAwait(false);
                }
            return items;
        }

        public async Task<IList<PaymentMethod>> ListUserPaymentMethods()
        {
            var url = Consts.GetUrl(Consts.ML_USER_PAYMENT_METHODS_URL, _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID));
            return await BaseServices<IList<PaymentMethod>>.GetAsync(url).ConfigureAwait(false);
        }
        #endregion

        #region Métodos referentes aos items
        public async Task<MLMyItemsSearchResult> ListMyItems(params KeyValuePair<string, object>[] attributes)
        {
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_MY_ITEMS_URL, _userId, _accessToken);
            return await BaseServices<MLMyItemsSearchResult>.GetAsync(url, attributes).ConfigureAwait(false);
        }

        public async Task<Item> GetItemDetails(string itemId, params KeyValuePair<string, object>[] attributes)
        {
            var url = Consts.GetUrl(Consts.ML_MY_ITEM_DETAIL, itemId);
            return await BaseServices<Item>.GetAsync(url, attributes).ConfigureAwait(false);
        }

        public async Task<MLCategorySearchResult> GetCategoryDetail(string categoryId)
        {
            var url = Consts.GetUrl(Consts.ML_CATEGORY_DETAILS, categoryId);
            return await BaseServices<MLCategorySearchResult>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLSearchResult> ListProductsByCategory(string categoryId, int pageIndex = 0, int pageSize = 0)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCTS_BY_CATEGORY, _settings.SelectedCountry, categoryId);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, --pageIndex));
            }
            return await BaseServices<MLSearchResult>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLSearchResult> ListProductsByName(string productName, int pageIndex = 0, int pageSize = 0)
        {
            var settings = SettingsService.Instance;
            var url = Consts.GetUrl(Consts.ML_PRODUCTS_BY_NAME, settings.SelectedCountry, productName, settings.UseAdultContent ?? false ? "yes" : "no");

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, (pageIndex * pageSize)));
            }
            return await BaseServices<MLSearchResult>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<ProductQuestionContent> AskQuestion(string question, string productId)
        {
            try
            {
                question = question.Replace(Environment.NewLine, "") + " - Enviado via Meu MercadoLivre Universal para Windows 10";
                _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_ASK_QUESTION_URL, productId, _accessToken);

                //Monta os parametros
                var parametros = new List<KeyValuePair<string, string>>();
                parametros.Add(new KeyValuePair<string, string>("item_id", productId));
                parametros.Add(new KeyValuePair<string, string>("text", question));


                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var jason = JsonConvert.SerializeObject(new { item_id = productId, text = question });

                var response = await _httpClient.PostAsync(url, new StringContent(jason));
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<ProductQuestionContent>(result));
                }
                return null;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.AskQuestion()", ex);
                return null;
            }
        }

        public async Task<MLProductDescription> GetProductDescrition(string productId)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCT_DESCRIPTION, productId);
            return await BaseServices<MLProductDescription>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<Item> ListNewItem(SellItem itemInfo)
        {
            try
            {
                var errorMessage = "";
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_LIST_ITEM, _accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var obj = new
                {
                    title = itemInfo.Title,
                    price = itemInfo.ProductValue,
                    available_quantity = itemInfo.Quantity,
                    condition = itemInfo.IsNew ?? false ? "new" : "used",
                    description = itemInfo.ProductDescription,
                    buying_mode = itemInfo.BuyingMode,
                    currency_id = itemInfo.CurrencyId,
                    category_id = itemInfo.ProductCategory,
                    listing_type_id = itemInfo.ListType.id

                };
                var json = new StringContent(JsonConvert.SerializeObject(obj));


                var response = await _httpClient.PostAsync(url, json);
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Item>(result));
                }
                else
                {
                    //Convertemos para o formato de erro
                    var error = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLErrorRequest>(result));

                    if (error?.cause != null)
                    {

                        await AppLogs.WriteLog("MercadoLivreServices.ListNewItem()", "Erro durante a criação do novo item", "");
                        if (error.cause != null)
                        {
                            foreach (var item in error.cause)
                            {
                                var cause = item as Cause;
                                errorMessage += cause?.message;
                                await AppLogs.WriteLog("     ", cause?.code + " - " + cause?.message, "");
                            }
                        }
                    }
                    if (!IsNullOrWhiteSpace(errorMessage))
                    {
                        await new MessageDialog(_resourceLoader
                                .GetString(Format("MercadoLivreServicesListNewItemErrorContent", errorMessage)),
                            _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                    }

                    await AppLogs.WriteWarning("MercadoLivreServices.ListNewItem()", result);
                }
                return null;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.ListNewItem()", ex);
                return null;
            }
        }

        /// <summary>
        /// Altera o status do produto
        /// </summary>
        /// <param name="productId">Codigo do produto</param>
        /// <param name="status">Status </param>        
        /// <returns></returns>
        public async Task<bool> ChangeProductStatus(string productId, MLProductStatus status)
        {
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_PRODUCT_CHANGE_STATUS, productId, _accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var jason = JsonConvert.SerializeObject(new
                {
                    status = status == MLProductStatus.mlpsActive ? "active" : status == MLProductStatus.mlpsPause ? "paused" : "closed"
                });
                var response = await _httpClient.PutAsync(url, new StringContent(jason));
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                await AppLogs.WriteWarning("MercadoLivreServices.GetProductStatus()", result);
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.GetProductStatus()", ex);
                return false;
            }
        }

        public async Task<bool> RemoveProduct(string productId)
        {
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_PRODUCT_REMOVE, productId, _accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var jason = JsonConvert.SerializeObject(new
                {
                    deleted = true
                });
                var response = await _httpClient.PutAsync(url, new StringContent(jason));
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }

                await AppLogs.WriteWarning("MercadoLivreServices.RemoveProduct()", result);
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.RemoveProduct()", ex);
                return false;
            }
        }

        public async Task<bool> RelistProduct(string productId, int quantity, double price, string listTypeId)
        {
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_PRODUCT_RELIST, productId, _accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                var jason = JsonConvert.SerializeObject(new
                {
                    quantity = quantity,
                    price = price,
                    listing_type_id = listTypeId
                });
                var response = await _httpClient.PostAsync(url, new StringContent(jason));
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    //Tentar converter para MLErrorRequest
                }
                await AppLogs.WriteWarning("MercadoLivreServices.RelistProduct()", result);
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.RelistProduct()", ex);
                return false;
            }
        }

        public async Task<bool> ChangeProductAttributes(string productId, dynamic attributes)
        {
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

                var url = Consts.GetUrl(Consts.ML_PRODUCT_CHANGE_ATTRIBUTES, productId, _accessToken);


                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                var jason = JsonConvert.SerializeObject(attributes);
                var response = await _httpClient.PutAsync(url, new StringContent(jason));
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                await AppLogs.WriteWarning("MercadoLivreServices.ChangeProductAttributes()", result);
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.ChangeProductAttributes()", ex);
                return false;
            }
        }

        public async Task<ShippingCost> GetShippingCost(string productId, string zipCode)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCT_CALCULATE_SHIPPING_COST, productId, zipCode);
            return await BaseServices<ShippingCost>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLProductVisits> GetProductVisits(string productId, DateTime startDate, DateTime finishDate)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCT_VISITS_URL, productId, startDate.ToString("yyyy-MM-dd"), finishDate.ToString("yyyy-MM-dd"));
            return await BaseServices<MLProductVisits>.GetAsync(url).ConfigureAwait(false);
        }

        #endregion

        #region Métodos referentes a autenticação
        public async Task<bool> RefreshAuthentication()
        {
            var token = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN);
            var clientSecret = Consts.ML_API_KEY;
            var clientId = Consts.ML_CLIENT_ID;

            var url = Consts.GetUrl(Consts.ML_URL_REFRESH_AUTHENTICATION, clientId, clientSecret, token);
            var result = await BaseServices<MLAutorizationInfo>.GetAsync(url).ConfigureAwait(false);
            return !IsNullOrWhiteSpace(result?.Access_Token);
        }
        #endregion

        #region Métodos referente aos pedidos
        public async Task<MLOrder> ListMyOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlMyordersListUrl, _userId, _accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            var result = await BaseServices<MLOrder>.GetAsync(url, attributes).ConfigureAwait(false);
            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "thumbnail") }).ConfigureAwait(false);
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        public async Task<MLOrder> ListOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlOrdersList, _userId, _accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            var result = await BaseServices<MLOrder>.GetAsync(url, attributes).ConfigureAwait(false);
            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>("attributes", "thumbnail")).ConfigureAwait(false);
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        public async Task<MLOrder> ListRecentOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {

            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlRecentOrdersList, _userId, _accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            var result = await BaseServices<MLOrder>.GetAsync(url, attributes).ConfigureAwait(false);

            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "thumbnail") });
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        public async Task<Feedback> GetOrderFeedback(string orderId)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.MlUrlOrderFeedback, orderId, _accessToken);
            return await BaseServices<Feedback>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLOrder> ListArchivedSellerOrders(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributes)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlUrlOrderSellerArchived, _userId, _accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }

            var result = await BaseServices<MLOrder>.GetAsync(url).ConfigureAwait(false);

            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "thumbnail") });
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        public async Task<MLOrder> ListArchivedBuyerOrders(int pageIndex = 0, int pageSize = 0,
            params KeyValuePair<string, object>[] attributes)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            var url = Consts.GetUrl(Consts.MlUrlOrderBuyerArchived, _userId, _accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, pageIndex));
            }
            var result = await BaseServices<MLOrder>.GetAsync(url, attributes).ConfigureAwait(false);

            if (result?.results != null)
            {
                foreach (var item in result.results)
                {
                    if (item.order_items != null)
                        foreach (var order in item.order_items)
                        {
                            if (order.item != null)
                            {
                                var thumbmnail =
                                    await
                                        GetItemDetails(order.item.id,
                                            new KeyValuePair<string, object>[]
                                            {new KeyValuePair<string, object>("attributes", "thumbnail")})
                                            .ConfigureAwait(false);
                                if (thumbmnail != null)
                                    order.item.thumbnail = thumbmnail.thumbnail;
                            }
                        }
                }
            }
            return result;
        }

        #endregion

        public async Task<IList<PaymentMethod>> ListPaymentMethods(string paisId)
        {
            var url = Consts.GetUrl(Consts.ML_PAYMENT_METHODS, _settings.SelectedCountry);
            return await BaseServices<IList<PaymentMethod>>.GetAsync(url).ConfigureAwait(false);
        }


        public async Task<Shipping> GetShippingDetails(string shipId)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.ML_SHIP_DETAILS, shipId, _accessToken);
            return await BaseServices<Shipping>.GetAsync(url).ConfigureAwait(false);
        }


        public async Task<MLMyItemsSearchResult> ListMyItems(string status, int pageIndex = 0, int pageSize = 0)
        {
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_MY_ITEMS_URL, _userId, _accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, --pageIndex));
            }

            url = Concat(url, Format("&status={0}", status));
            return await BaseServices<MLMyItemsSearchResult>.GetAsync(url).ConfigureAwait(false);
        }


        public async Task<MLMyItemsSearchResult> ListMyItems(int pageIndex = 0, int pageSize = 0, params KeyValuePair<string, object>[] attributesAndFilters)
        {
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_MY_ITEMS_URL, _userId, _accessToken);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, (pageIndex * pageSize)));
            }

            return await BaseServices<MLMyItemsSearchResult>.GetAsync(url, attributesAndFilters).ConfigureAwait(false);
        }


        public async Task<IList<MLListType>> ListTypes(string countryId)
        {
            _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Consts.GetUrl(Consts.ML_LIST_TYPE, countryId);
            return await BaseServices<IList<MLListType>>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<MLAutorizationInfo> TryRefreshToken()
        {
            try
            {
                var refreshToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_REFRESH_TOKEN);
                var url = Consts.GetUrl(Consts.ML_REFRESH_TOKEN_URL, Consts.ML_CLIENT_ID, Consts.ML_API_KEY, refreshToken);


                var response = await _httpClient.PostAsync(url, new StringContent(Empty)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<MLAutorizationInfo>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MeuMercadoLivreServices.TryRefreshToken()", ex);
                return null;
            }
        }

        public async Task<bool> BookmarkItem(string itemId)
        {
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_PRODUCT_BOOKMARK_URL, _accessToken);

                var jason = JsonConvert.SerializeObject(new { item_id = itemId });
                var response = await _httpClient.PostAsync(url, new StringContent(jason)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.BookmarkItem()", ex);
                return false;
            }
        }

        public async Task<bool> RemoveBookmarkItem(string itemId)
        {
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_PRODUCT_BOOKMARK_REMOVE_URL, itemId, _accessToken);

                var jason = JsonConvert.SerializeObject(new { item_id = itemId });
                var response = await _httpClient.DeleteAsync(url).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.RemoveBookmarkItem()", ex);
                return false;
            }
        }

        public async Task<MLOrderInfo> GetOrderDetail(string orderId)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
            var url = Consts.GetUrl(Consts.MlOrderDetails, orderId, _accessToken);
            var result = await BaseServices<MLOrderInfo>.GetAsync(url).ConfigureAwait(false);
            if (result?.order_items != null)
            {
                foreach (var order in result.order_items)
                {
                    if (order.item != null)
                    {
                        var thumbmnail = await GetItemDetails(order.item.id, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("attributes", "thumbnail") }).ConfigureAwait(false);
                        if (thumbmnail != null)
                            order.item.thumbnail = thumbmnail.thumbnail;
                    }
                }
            }
            return result;
        }

        public async Task<IReadOnlyList<MLListPrice>> GetListingPrices(string countryId, double productPrice, params KeyValuePair<string, object>[] attributesOrFilters)
        {
            DecimalFormatter formatter = new DecimalFormatter(new string[] { "en-US" }, "US");

            var stringPrice = formatter.Format(productPrice);
            var url = Consts.GetUrl(Consts.ML_LIST_PRICES, countryId, stringPrice);

            return await BaseServices<IReadOnlyList<MLListPrice>>.GetAsync(url, attributesOrFilters).ConfigureAwait(false);
        }

        public async Task<bool> RevokeAccess()
        {
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                _userId = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var url = Format(Consts.ML_REVOKE_LOGIN, _userId, Consts.ML_CLIENT_ID, _accessToken);
                var r = await _httpClient.DeleteAsync(url);

                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var s = await r.Content.ReadAsStringAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.RevokeAccess()", ex);
                return false;
            }
        }

        public async Task<IReadOnlyList<MLListType>> GetAvailableUpgrades(string itemId)
        {
            _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);

            var url = Format(Consts.ML_LIST_ITEM_UPGRADES, itemId, _accessToken);
            return await BaseServices<IReadOnlyList<MLListType>>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<bool> ChangeItemListType(string itemId, string id_type)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_ITEM_CHANGE_LIST_TYPE, itemId, _accessToken);

                var jason = JsonConvert.SerializeObject(new { id = id_type });
                var response = await _httpClient.PostAsync(url, new StringContent(jason)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.RemoveBookmarkItem()", ex);
                return false;
            }
        }


        public async Task<bool> SendSellerOrderFeedback(string orderId, bool fulfilled, MLRating rating, string message, MLSellerRatingReason reason, bool restockItem = false, bool hasSellerRefundMoney = false)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.MlUrlOrderFeedback, orderId, _accessToken);


                var jason = Empty;
                if (fulfilled)
                {
                    jason = JsonConvert.SerializeObject(new
                    {
                        fulfilled = fulfilled,
                        rating = Enum.GetName(typeof(MLRating), rating),
                        message = message
                    });
                }
                else
                {
                    jason = JsonConvert.SerializeObject(new
                    {
                        fulfilled = fulfilled,
                        rating = Enum.GetName(typeof(MLRating), rating),
                        message = message,
                        reason = Enum.GetName(typeof(MLSellerRatingReason), reason),
                        restock_item = restockItem,
                        has_seller_refunded_money = hasSellerRefundMoney
                    });
                }
                var response = await _httpClient.PostAsync(url, new StringContent(jason)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.SendSellerOrderFeedback()", ex);
                return false;
            }
        }

        public async Task<bool> SendBuyerOrderFeedback(string orderId, bool fulfilled, MLRating rating, string message, MLBuyerRatingReason reason)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.MlUrlOrderFeedback, orderId, _accessToken);


                var jason = Empty;
                if (fulfilled)
                {
                    jason = JsonConvert.SerializeObject(new
                    {
                        fulfilled = fulfilled,
                        rating = Enum.GetName(typeof(MLRating), rating),
                        message = message
                    });
                }
                else
                {
                    jason = JsonConvert.SerializeObject(new
                    {
                        fulfilled = fulfilled,
                        rating = Enum.GetName(typeof(MLRating), rating),
                        message = message,
                        reason = Enum.GetName(typeof(MLBuyerRatingReason), reason)
                    });
                }
                var response = await _httpClient.PostAsync(url, new StringContent(jason)).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.SendBuyerOrderFeedback()", ex);
                return false;
            }
        }
        public async Task<MLImage> UploadProductImage(ProductImage image)
        {
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_UPLOAD_IMAGE, _accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent content = new StringContent("file");
                form.Add(content, "file");
                var file = await StorageFile.GetFileFromPathAsync(image.LocalPath);
                var stream = await file.OpenStreamForReadAsync();
                content = new StreamContent(stream);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = file.Name
                };

                form.Add(content);
                var response = await _httpClient.PostAsync(url, form);

                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLImage>(result));
                }
                return null;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.UploadProductImage()", ex);
                return null;
            }


        }
        public async Task<bool> AddPicture(string pictureId, string itemId)
        {
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_ITEM_POST_IMAGE, itemId, _accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var obj = new
                {
                    id = pictureId
                };
                var json = new StringContent(JsonConvert.SerializeObject(obj));

                var response = await _httpClient.PostAsync(url, json);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.AddPicture()", ex);
                return false;
            }
        }

        public async Task<MLErrorRequest> ValidateNewItem(SellItem itemInfo)
        {
            //
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_VALIDATE_NEW_ITEM, _accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var obj = new
                {
                    title = itemInfo.Title,
                    price = itemInfo.ProductValue,
                    available_quantity = itemInfo.Quantity,
                    condition = itemInfo.IsNew ?? false ? "new" : "used",
                    description = itemInfo.ProductDescription,
                    buying_mode = itemInfo.BuyingMode,
                    currency_id = itemInfo.CurrencyId,
                    category_id = itemInfo.ProductCategory,
                    listing_type_id = itemInfo.ListType.id

                };
                var json = new StringContent(JsonConvert.SerializeObject(obj));


                var response = await _httpClient.PostAsync(url, json).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return null;
                }
                else
                {
                    //Convertemos para o formato de erro
                    var error = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLErrorRequest>(result)).ConfigureAwait(false);

                    if (error != null)
                    {                        
                        await AppLogs.WriteLog("MercadoLivreServices.ValidateNewItem()", "Erro durante a criação do novo item", "").ConfigureAwait(false);
                        if (error.cause != null)
                            foreach (var item in error.cause)
                            {
                                var cause = item as Cause;
                                await AppLogs.WriteLog("     ", cause?.code + " - " + cause?.message, "");
                            }
                    }

                    await AppLogs.WriteWarning("MercadoLivreServices.ValidateNewItem()", result);
                }
                return null;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.ValidateNewItem()", ex);
                return null;
            }
        }


        public async Task<bool> ChangeProductDescription(string productId, string text)
        {
            //
            try
            {
                _accessToken = _dataService.GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var url = Consts.GetUrl(Consts.ML_CHANGE_PRODUCT_DESCRIPTION, productId, _accessToken);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var obj = new
                {
                    text = text
                };
                var json = new StringContent(JsonConvert.SerializeObject(obj));


                var response = await _httpClient.PutAsync(url, json).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    //Convertemos para o formato de erro
                    var error = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MLErrorRequest>(result)).ConfigureAwait(false);

                    if (error != null)
                    {

                        await AppLogs.WriteLog("MercadoLivreServices.ChangeProductDescription()", "Erro durante a criação do novo item", "").ConfigureAwait(false);
                        if (error.cause != null)
                            foreach (var item in error.cause)
                            {
                                var cause = item as Cause;
                                await AppLogs.WriteLog("     ", cause?.code + " - " + cause?.message, "");
                            }
                    }

                    await AppLogs.WriteWarning("MercadoLivreServices.ListNewItem()", result);
                }
                return false;
            }
            catch (Exception ex)
            {
                await AppLogs.WriteError("MercadoLivreServices.ChangeProductDescription()", ex);
                return false;
            }
        }

        public async Task<MLSearchResult> ListProductsByUser(string _userId, int pageIndex = 0, int pageSize = 0)
        {
            var url = Consts.GetUrl(Consts.ML_PRODUCTS_BY_USER, _settings.SelectedCountry, _userId);

            if (pageIndex >= 0 && pageSize > 0)
            {
                url = Concat(url, Format("&limit={0}&offset={1}", pageSize, (pageIndex * pageSize)));
            }
            return await BaseServices<MLSearchResult>.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<Country>> ListCountries()
        {
            var url = Consts.ML_URL_LIST_SITES;
            
            var result = await BaseServices<IReadOnlyCollection<Country>>.GetAsync(url).ConfigureAwait(false);
            var orderedResult = result.OrderBy(c => c.name);
            return orderedResult.ToList();
        }

        public async Task<IReadOnlyCollection<MLItemHomeFeature>> ListFeaturedHomeItems()
        {
            return await BaseServices<IReadOnlyCollection<MLItemHomeFeature>>
                .GetAsync(Format(Consts.MlUrlDestaquesHome, _settings.SelectedCountry)).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<MLItemHomeFeature>> ListFeaturedCategoryItems(string categoryId)
        {
            return await BaseServices<IReadOnlyCollection<MLItemHomeFeature>>
                .GetAsync(Format(Consts.MlUrlDestaquesCategoria, categoryId, _settings.SelectedCountry))
                .ConfigureAwait(false);
        }
    }
}
