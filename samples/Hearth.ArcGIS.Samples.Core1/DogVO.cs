using ArcGIS.Desktop.Framework.Contracts;

namespace Hearth.ArcGIS.Samples.Core1
{
    public class DogVO : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private int _age;
        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }
        private DateTime _birthday;
        public DateTime Birthday
        {
            get => _birthday;
            set => SetProperty(ref _birthday, value);
        }
    }
}