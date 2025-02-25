using ArcGIS.Desktop.Framework.Dialogs;
using AutoMapper;
using Hearth.ArcGIS.Samples.Core1;
using Hearth.ArcGIS.Samples.Core2;
using Hearth.ArcGIS.Samples.PlugIns.Contracts;
using System.Text.Json;

namespace Hearth.ArcGIS.Samples.PlugIns.Menus
{
    internal class AutoMapperSampleButton : InjectableButton
    {
        [Inject]
        private readonly IMapper? _mapper;

        protected override void OnClick()
        {
            Person person = new Person
            {
                Id = Guid.NewGuid(),
                Age = 30,
                Name = "John Doe",
                Birthday = DateTime.Now
            };
            PersonVO? personVO = _mapper?.Map<PersonVO>(person);
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string personJson = JsonSerializer.Serialize(personVO, options);
            if (personVO != null)
            {
                MessageBox.Show(personJson, "AutoMapper Sample");
            }

            DogVO dogVO = new DogVO
            {
                Age = 5,
                Name = "Rufus",
                Birthday = DateTime.Now
            };
            Dog? dog = _mapper?.Map<Dog>(dogVO);
            string dogJson = JsonSerializer.Serialize(dog, options);
            if (dog != null)
            {
                MessageBox.Show(dogJson, "AutoMapper Sample");
            }
        }
    }
}
