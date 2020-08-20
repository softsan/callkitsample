using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallKitSample.Services;
using Xamarin.Forms;

namespace CallKitSample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private string _callerName;
        public string CallerName
        {
            get => _callerName;
            set
            {
                _callerName = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            var service = DependencyService.Get<ICallService>();
            service.StartCall(CallerName);
        }

        void EndCall(System.Object sender, System.EventArgs e)
        {
            var service = DependencyService.Get<ICallService>();
            service.EndCall();
        }
    }
}
