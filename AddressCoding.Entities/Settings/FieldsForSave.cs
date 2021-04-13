using GalaSoft.MvvmLight;

namespace AddressCoding.Entities.Settings
{
    public class FieldsForSave: ViewModelBase
    {
        private bool _canSave = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CanSave
        {
            get => _canSave;
            set => Set(ref _canSave, value);
        }

        private string _name = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _description = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }
    }
}