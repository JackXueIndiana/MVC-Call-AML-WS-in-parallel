using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleSearchMVCApp.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Search5(float soot, 
            float kv100,
            float aluminum,
            float chromium,
            float copper,
            float iron,
            float lead,
            float tin,
            float nickel,
            float silicon,
            float sodium,
            float potassium,
            float miles)
        {
            List<Sample> samples = new List<Sample>();

            var scoreRequest = new
            {
                Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                            {
                                                "SOOT_F", soot.ToString()
                                            },
                                            {
                                                "KV100_F", kv100.ToString()
                                            },
                                            {
                                                "ALUMINUM", aluminum.ToString()
                                            },
                                            {
                                                "CHROMIUM", chromium.ToString()
                                            },
                                            {
                                                "COPPER", copper.ToString()
                                            },
                                            {
                                                "IRON", iron.ToString()
                                            },
                                            {
                                                "LEAD", lead.ToString()
                                            },
                                            {
                                                "TIN", tin.ToString()
                                            },
                                            {
                                                "NICKEL", nickel.ToString()
                                            },
                                            {
                                                "SILICON", silicon.ToString()
                                            },
                                            {
                                                "SODIUM", sodium.ToString()
                                            },
                                            {
                                                "POTASSIUM", potassium.ToString()
                                            },
                                            {
                                                "MILES", miles.ToString()
                                            },
                                            {
                                                "FC_I", "0"
                                            },
                                }
                            }
                        },
                    },
                GlobalParameters = new Dictionary<string, string>()
                {
                }
            };

            string irclKey = "sIygQYGmVOWOi4/dYj0+HWdLkHe+f5hdC+3UAogjwqcHC+f8uNpjAV9RuJgAJWwie+tqQFQjntpKW8MkJ10CsA==";
            string irclUrl = "https://ussouthcentral.services.azureml.net/workspaces/e261c2c53b704717a54bbdfef8377355/services/fec126d7a788425aa01f742a130a2c8a/execute?api-version=2.0&format=swagger";

            string bpxKey = "d1aLKsnyW5w8EA8VR8org4eFSoNkcUZJRjMdFCyX2VxtIEqkg3rGJ5Bk9hg70MxhbANqrqseAn0YK3L3RPJESg==";
            string bpxUrl = "https://ussouthcentral.services.azureml.net/workspaces/e261c2c53b704717a54bbdfef8377355/services/6a34bfda80124a089f4d30b2a592bfff/execute?api-version=2.0&format=swagger";

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bpxKey);
                    client.BaseAddress = new Uri(bpxUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    
                    var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);
                    var output1 = jsonResult["Results"]["output1"][0];
                    Sample sample = new Sample();
                    /*sample.soot = output1["SOOT_F"];
                    sample.kv100 = output1["KV100_F"];
                    sample.aluminum = output1["ALUMINUM"];
                    sample.chromium = output1["CHROMIUM"];
                    sample.copper = output1["COPPER"];
                    sample.iron = output1["IRON"];
                    sample.lead = output1["LEAD"];
                    sample.tin = output1["TIN"];
                    sample.nickel = output1["NICKEL"];
                    sample.silicon = output1["SILICON"];
                    sample.sodium = output1["SODIUM"];
                    sample.potassium = output1["POTASSIUM"];
                    sample.miles = output1["MILES"];*/
                    sample.fc = "BPX";
                    sample.fci = 0;
                    sample.scoreLable = output1["Scored Labels"];
                    sample.probability = output1["Scored Probabilities"];

                    samples.Add(sample);
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", irclKey);
                    client.BaseAddress = new Uri(irclUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);
                    var output1 = jsonResult["Results"]["output1"][0];
                    Sample sample = new Sample();
                    /*sample.soot = output1["SOOT_F"];
                    sample.kv100 = output1["KV100_F"];
                    sample.aluminum = output1["ALUMINUM"];
                    sample.chromium = output1["CHROMIUM"];
                    sample.copper = output1["COPPER"];
                    sample.iron = output1["IRON"];
                    sample.lead = output1["LEAD"];
                    sample.tin = output1["TIN"];
                    sample.nickel = output1["NICKEL"];
                    sample.silicon = output1["SILICON"];
                    sample.sodium = output1["SODIUM"];
                    sample.potassium = output1["POTASSIUM"];
                    sample.miles = output1["MILES"];*/
                    sample.fc = "IRCL";
                    sample.fci = 0;
                    sample.scoreLable = output1["Scored Labels"];
                    sample.probability = output1["Scored Probabilities"];

                    samples.Add(sample);
                }
            }
            catch (Exception e)
            {
                throw (e);
            }

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = samples
            };
        }

        async Task<Sample> PerformAMLWSCall(string url, string key,
            float soot,
            float kv100,
            float aluminum,
            float chromium,
            float copper,
            float iron,
            float lead,
            float tin,
            float nickel,
            float silicon,
            float sodium,
            float potassium,
            float miles,
            string fc)
        {
            HttpClient client = new HttpClient();
            var scoreRequest = new
            {
                Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                            {
                                                "SOOT_F", soot.ToString()
                                            },
                                            {
                                                "KV100_F", kv100.ToString()
                                            },
                                            {
                                                "ALUMINUM", aluminum.ToString()
                                            },
                                            {
                                                "CHROMIUM", chromium.ToString()
                                            },
                                            {
                                                "COPPER", copper.ToString()
                                            },
                                            {
                                                "IRON", iron.ToString()
                                            },
                                            {
                                                "LEAD", lead.ToString()
                                            },
                                            {
                                                "TIN", tin.ToString()
                                            },
                                            {
                                                "NICKEL", nickel.ToString()
                                            },
                                            {
                                                "SILICON", silicon.ToString()
                                            },
                                            {
                                                "SODIUM", sodium.ToString()
                                            },
                                            {
                                                "POTASSIUM", potassium.ToString()
                                            },
                                            {
                                                "MILES", miles.ToString()
                                            },
                                            {
                                                "FC_I", "0"
                                            },
                                }
                            }
                        },
                    },
                GlobalParameters = new Dictionary<string, string>()
                {
                }
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
            string responseBody = response.Content.ReadAsStringAsync().Result;

            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);
            var output1 = jsonResult["Results"]["output1"][0];
            Sample sample = new Sample();
            sample.fc = fc;
            sample.fci = 0;
            sample.scoreLable = output1["Scored Labels"];
            sample.probability = output1["Scored Probabilities"];

            return sample;
        }

        async Task<RelatedFCs> PerformAMLWSRelatedCodesCall(string url, string key, string fc)
        {
            HttpClient client = new HttpClient();
            var scoreRequest = new
            {
                Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                             {
                                                "Item", fc
                                            },

                                }
                            }
                        },
                    },
                GlobalParameters = new Dictionary<string, string>()
                {
                }
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
            string responseBody = response.Content.ReadAsStringAsync().Result;

            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);
            var output1 = jsonResult["Results"]["output1"][0];
            RelatedFCs codes = new RelatedFCs();
            codes.fc = fc;
            string x = output1["Related Item 1"];
            if (x != null)
            {
                codes.rfc1 = x;
            }
            x = output1["Related Item 2"];
            if (x != null)
            {
                codes.rfc2 = x;
            }
            x = output1["Related Item 3"];
            if (x != null)
            {
                codes.rfc3 = x;
            }
            x = output1["Related Item 4"];
            if (x != null)
            {
                codes.rfc4 = x;
            }
            x = output1["Related Item 5"];
            if (x != null)
            {
                codes.rfc5 = x;
            }
            return codes;
        }
        public async Task<ActionResult> Search(float soot,
            float kv100,
            float aluminum,
            float chromium,
            float copper,
            float iron,
            float lead,
            float tin,
            float nickel,
            float silicon,
            float sodium,
            float potassium,
            float miles)
        {
            List<Sample> samples = new List<Sample>();
            
            string irclKey = "sIygQYGmVOWOi4/dYj0+HWdLkHe+f5hdC+3UAogjwqcHC+f8uNpjAV9RuJgAJWwie+tqQFQjntpKW8MkJ10CsA==";
            string irclUrl = "https://ussouthcentral.services.azureml.net/workspaces/e261c2c53b704717a54bbdfef8377355/services/fec126d7a788425aa01f742a130a2c8a/execute?api-version=2.0&format=swagger";

            string bpxKey = "d1aLKsnyW5w8EA8VR8org4eFSoNkcUZJRjMdFCyX2VxtIEqkg3rGJ5Bk9hg70MxhbANqrqseAn0YK3L3RPJESg==";
            string bpxUrl = "https://ussouthcentral.services.azureml.net/workspaces/e261c2c53b704717a54bbdfef8377355/services/6a34bfda80124a089f4d30b2a592bfff/execute?api-version=2.0&format=swagger";

            Task<Sample> sBpx = PerformAMLWSCall(bpxUrl, bpxKey,
                soot,
            kv100,
            aluminum,
            chromium,
            copper,
            iron,
            lead,
            tin,
            nickel,
            silicon,
            sodium,
            potassium,
            miles,
                "BPX");

            Task<Sample> sIrcl = PerformAMLWSCall(irclUrl, irclKey,
                soot,
            kv100,
            aluminum,
            chromium,
            copper,
            iron,
            lead,
            tin,
            nickel,
            silicon,
            sodium,
            potassium,
            miles,
                "IRCL");

            Sample aBpx = await sBpx;
            Sample aIrlc = await sIrcl;

            if (aBpx != null)
            {
                samples.Add(aBpx);
            }
            if (aIrlc != null)
            {
                samples.Add(aIrlc);
            }
            
            if (samples != null)
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = samples
                };
            }
            else
            {
                return View();
            }
        }

        public async Task<ActionResult> ClickSearch(string fc)
        {
            string rcKey = "C4cxARtgz3jQBfO3Mi38i5dF0mJRpYJCvhfEJY7FhWD+LOLWmLrSbTEOxrjk374Vs5Y1ia6Sphqfv34jCk+v1g==";
            string rcUrl = "https://ussouthcentral.services.azureml.net/workspaces/e261c2c53b704717a54bbdfef8377355/services/6e5eddfd02ea462dad65c3917d919122/execute?api-version=2.0&format=swagger";

            Task<RelatedFCs> sRc = PerformAMLWSRelatedCodesCall(rcUrl, rcKey, fc);
            RelatedFCs aRc = await sRc;

            if (aRc != null)
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = aRc
                };
            }
            else
            {
                return View();
            }
        }
    }
}
