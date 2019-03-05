using AddressCoding.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressCoding.Repository
{
    public class OrponRepository : IRepository
    {
        private string _address;
        private string _nameEndpoint;

        private Orpon.wsSearchAddrElByFullNamePortTypeClient _client;

        public EntityResult<Entities.Orpon> GetOrpon(string data)
        {
            EntityResult<Entities.Orpon> result = new EntityResult<Entities.Orpon>();

            try
            {
                _client = new Orpon.wsSearchAddrElByFullNamePortTypeClient(_nameEndpoint, _address);

                var add = new Orpon.AddressElementNameDataAddressElementFullNameGroup[]
                {
                    new Orpon.AddressElementNameDataAddressElementFullNameGroup()
                    {
                        FullAddress = data
                    }
                };

                var r = _client.SearchAddressElementByFullName(new Orpon.AddressElementNameData()
                {
                    AddressElementFullNameList = add
                });

                if (r != null)
                {
                    if (r.AddressElementResponseList.Length > 0)
                    {
                        var a = r.AddressElementResponseList[0];
                        result.Object = new Entities.Orpon()
                        {
                            Building = a.Building,
                            BuildingBlock = a.BuildingBlock,
                            BuildingBlockLitera = a.BuildingBlockLitera,
                            BuildingLitera = a.BuildingLitera,
                            CheckStatus = a.CheckStatus,
                            CornerHouse = a.CornerHouse,
                            FIASHouseId = a.FIASHouseId,
                            FIASLocalityId = a.FIASLocalityId,
                            FIASStreetId = a.FIASStreetId,
                            GlobalID = a.GlobalID,
                            House = a.House,
                            HouseLitera = a.HouseLitera,
                            KLADRLocalityId = a.KLADRLocalityId,
                            KLADRStreetId = a.KLADRStreetId,
                            Ownership = a.Ownership,
                            OwnershipLitera = a.OwnershipLitera,
                            ParsingLevelCode = a.ParsingLevelCode,
                            QualityCode = a.QualityCode,
                            Street = a.Street,
                            StreetKind = a.StreetKind,
                            SystemCode = a.SystemCode
                        };
                    }
                    else
                    {
                        throw new Exception("Error AddressElementResponseList.Count = 0");
                    }
                }
                else
                {
                    result.Result = false;
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
                result.Result = false;
            }
            finally
            {
                _client.Close();
            }

            return result;
        }

        public EntityResult<IEnumerable<Entities.Orpon>> GetOrpon(IEnumerable<string> data)
        {
            throw new NotImplementedException();
        }

        public Task<EntityResult<Entities.Orpon>> GetOrponAsync(string data)
        {
            return Task.Run(() => GetOrpon(data));
        }

        public Task<EntityResult<IEnumerable<Entities.Orpon>>> GetOrponAsync(IEnumerable<string> data)
        {
            throw new NotImplementedException();
        }

        public void Initialize(string address, string endPoint)
        {
            _address = address;
            _nameEndpoint = endPoint;
        }

        public Task<EntityResult<string>> TestConnectAsync()
        {
            return Task.Run(() =>
            {
                EntityResult<string> result = new EntityResult<string>();
                try
                {
                    _client = new Orpon.wsSearchAddrElByFullNamePortTypeClient(_nameEndpoint, _address);

                    _client.Open();

                    result.Result = true;
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                    result.Result = false;
                }
                finally
                {
                    if(_client!=null)
                    {
                        _client.Close();
                    }
                }
                
                return result;
            });
        }
    }
}