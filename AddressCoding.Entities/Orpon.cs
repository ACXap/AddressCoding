using GalaSoft.MvvmLight;

namespace AddressCoding.Entities
{
    public class Orpon :ViewModelBase
    {
        private string _qualityCode = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string QualityCode
        {
            get => _qualityCode;
            set => Set(ref _qualityCode, value);
        }

        private string _checkStatus = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string CheckStatus
        {
            get => _checkStatus;
            set => Set(ref _checkStatus, value);
        }

        private string _parsingLevelCode = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ParsingLevelCode
        {
            get => _parsingLevelCode;
            set => Set(ref _parsingLevelCode, value);
        }

        private string _systemCode = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string SystemCode
        {
            get => _systemCode;
            set => Set(ref _systemCode, value);
        }


        private string _globalID = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string GlobalID
        {
            get => _globalID;
            set => Set(ref _globalID, value);
        }

        private string _kLADRLocalityId = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string KLADRLocalityId
        {
            get => _kLADRLocalityId;
            set => Set(ref _kLADRLocalityId, value);
        }

        private string _fIASLocalityId = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string FIASLocalityId
        {
            get => _fIASLocalityId;
            set => Set(ref _fIASLocalityId, value);
        }

        private string _kLADRStreetId = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string KLADRStreetId
        {
            get => _kLADRStreetId;
            set => Set(ref _kLADRStreetId, value);
        }

        private string _fIASStreetId = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string FIASStreetId
        {
            get => _fIASStreetId;
            set => Set(ref _fIASStreetId, value);
        }

        private string _street = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Street
        {
            get => _street;
            set => Set(ref _street, value);
        }

        private string _streetKind = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string StreetKind
        {
            get => _streetKind;
            set => Set(ref _streetKind, value);
        }

        private string _house = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string House
        {
            get => _house;
            set => Set(ref _house, value);
        }

        private string _houseLitera = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string HouseLitera
        {
            get => _houseLitera;
            set => Set(ref _houseLitera, value);
        }

        private string _cornerHouse = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string CornerHouse
        {
            get => _cornerHouse;
            set => Set(ref _cornerHouse, value);
        }

        private string _buildingBlock = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string BuildingBlock
        {
            get => _buildingBlock;
            set => Set(ref _buildingBlock, value);
        }

        private string _buildingBlockLitera = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string BuildingBlockLitera
        {
            get => _buildingBlockLitera;
            set => Set(ref _buildingBlockLitera, value);
        }

        private string _building = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Building
        {
            get => _building;
            set => Set(ref _building, value);
        }

        private string _buildingLitera = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string BuildingLitera
        {
            get => _buildingLitera;
            set => Set(ref _buildingLitera, value);
        }

        private string _ownership = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Ownership
        {
            get => _ownership;
            set => Set(ref _ownership, value);
        }

        private string _ownershipLitera = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string OwnershipLitera
        {
            get => _ownershipLitera;
            set => Set(ref _ownershipLitera, value);
        }

        private string _fIASHouseId = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string FIASHouseId
        {
            get => _fIASHouseId;
            set => Set(ref _fIASHouseId, value);
        }
    }
}