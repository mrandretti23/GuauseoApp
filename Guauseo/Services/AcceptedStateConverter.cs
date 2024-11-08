using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Guauseo.Services
{
    public class AcceptedStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string estado && estado == "Aceptado")
            {
                return true;
            }
            if (value is string estado2 && estado2 == "En camino")
            {
                return true;
            }
            if (value is string estado3 && estado3 == "Estoy Afuera")
            {
                return true;
            }
            if (value is string estado4 && estado4 == "Paseando")
            {
                return true;
            }
            if (value is string estado5 && estado5 == "Estoy de regreso")
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

