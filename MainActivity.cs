using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppLoLRepo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public class Champion
        {
            public Champion(string name, string lore, string id, string image)
            {
                Name = name;
                Lore = lore;
                Id = id;
                Image = image;
            }

            public string Name { get; set; }
            public string Lore { get; set; }
            public string Id { get; set; }
            public string Image { get; set; }
        }

        private RecyclerView _recyclerView;
        // List<string> championImageUrls = new List<string>();
        private readonly List<Champion> _championList = new List<Champion>();



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            GetData();
            SetupRecyclerView();
        }

        private void SetupRecyclerView()
        {
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(_recyclerView.Context));
            var championAdapter = new ChampionAdapter(_championList);
            _recyclerView.SetAdapter(championAdapter);
        }


        private async void GetData()
        {
            // Get latest version
            const string versionUrl = "https://ddragon.leagueoflegends.com/api/versions.json";
            string latest;

            using (var client = new HttpClient())
            using (var response = client.GetAsync(versionUrl).Result)
            using (var content = response.Content)
            {
                var result = await content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<List<string>>(result);
                latest = json[0];
            }

            // Get champion JSON
            if (latest == null) return;
            var championUrl = "https://ddragon.leagueoflegends.com/cdn/" + latest + "/data/en_US/champion.json";

            using (var client = new HttpClient())
            using (var response = client.GetAsync(championUrl).Result)
            using (var content = response.Content)
            {
                var result = await content.ReadAsStringAsync();
                var json = JObject.Parse(result);

                const string baseUrl = "http://ddragon.leagueoflegends.com/cdn/img/champion/splash/";
                const string end = "_0.jpg";

                var championData = json["data"].ToList();

                foreach (var token in championData)
                {
                    var champion = token.ToObject<JProperty>();
                    if (champion == null) continue;
                    //championImageUrls.Add(baseUrl + champion.Name + end);
                    _championList.Add(new Champion(
                        champion.Value["name"]?.ToString(),
                        champion.Value["blurb"]?.ToString(),
                        champion.Value["id"]?.ToString(),
                        baseUrl + champion.Name + end
                    ));
                }
            }
        }
    }
}