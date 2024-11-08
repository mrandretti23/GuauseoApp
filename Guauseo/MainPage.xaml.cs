namespace Guauseo
{
    public partial class MainPage : ContentPage
    {
        

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClickedDueño(object sender, EventArgs e)
        {

            //Pone el titulo de la pagina 
            //await Navigation.PushAsync(new pgn_loginDueño());

            //No pone el titulo de la pagina 
            await Navigation.PushModalAsync(new Views.LoginDueñoView());

        }

        private void btn_Paseador_ClickedPaseador(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new Views.LoginPaseadorView());
        }

    }

}
