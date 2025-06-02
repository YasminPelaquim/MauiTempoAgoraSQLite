using System.Collections.ObjectModel;
using MauiTempoAgoraSQLite;
using MauiTempoAgoraSQLite.Models;
using MauiTempoAgoraSQLite.Services;

namespace MauiTempoAgoraSQLite

{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Tempo> lista = new ObservableCollection<Tempo>();

        public string cidade;

        public MainPage()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = lista;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Temp Máx: {t.temp_max} \n" +
                                         $"Temp Min: {t.temp_min} \n";

                        lbl_res.Text = dados_previsao;

                        string mapa = $"https://embed.windy.com/embed.html?" +
                                      $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                                      $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                      $"&lat={t.lat.ToString().Replace(",", ".")}&lon=" +
                                      $"{t.lon.ToString().Replace(",", ".")}";

            
                    }
                    else
                    {
                        lbl_res.Text = "Sem dados de Previsão";
                    }// fecha if t=null
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }// fecha if string is null or empty
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private async void GetCidade(double lat, double lon)
        {
            try
            {
                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);

                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da Cidade", ex.Message, "OK");
            }
        }

        private async void lst_produtos_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lista.Clear();                

                List<Tempo> tmp = await App.Db.GetAll();
                
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecionado = sender as MenuItem;

                Tempo p = selecionado.BindingContext as Tempo;

                bool confirm = await DisplayAlert("Tem Certeza?", $"Remover {p.description}?", "Sim", "Não");

                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    lista.Remove(p);
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                lista.Clear();

                List<Tempo> tmp = await App.Db.GetAll();

                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }

}