using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Threading.Tasks;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }
    
        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await  DataService.GetPrevisao(txt_cidade.Text);
                    if (t != null)
                    {
                        string dados_previsao = "";
                        dados_previsao  = $"Latitude: {t.lat} \n"+
                            $"Longitude: {t.lon} \n" +
                            $"Nascer do Sol: {t.sunrise} \n" +
                            $"Por do Sol: {t.sunset} \n" +
                            $"Temp Máx: {t.temp_max} \n" +
                            $"Temp Min: {t.temp_min} \n";
                        lbl_res.Text = dados_previsao;
                    }
                    else
                    {
                        lbl_res.Text = "Sem dados de Previsão";
                    }
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private async void  Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = 
                    new GeolocationRequest(GeolocationAccuracy.Low, TimeSpan.FromSeconds(10));

                Location? local = await Geolocation.Default.GetLocationAsync(request);
                if (local != null)
                {
                    string  local_disp = $"Latitude: {local.Latitude} \n"+
                        $"Longitude {local.Longitude} \n";
                    lbl_coords.Text = local_disp;
                    //Pega o nome da cidade que está nas cordenadas.
                    GetCidade(local.Latitude, local.Longitude);
                }
                else
                {
                    lbl_coords.Text = "Nenhuma Localização";
                }
            }
            catch(FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não Suporta", fnsEx.Message, "OK");
            }
            catch(FeatureNotEnabledException fneEx) 
            {
                await DisplayAlert("Erro: Localização desabilitada. ", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permisão de localização", pEx.Message, "OK");
            }catch(Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "Ok");
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
            } catch (Exception ex) {
                await DisplayAlert("Erro: Obtenção do nome da cidade", ex.Message, "OK");   
            }
        }
    }
}
