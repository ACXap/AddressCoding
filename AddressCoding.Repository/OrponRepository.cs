using AddressCoding.Entities;
using AddressCoding.Repository.Orpon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AddressCoding.Repository
{
    public class OrponRepository : IRepository
    {
        private string _address;
        private string _nameEndpoint;

        public EntityResult<Entities.Orpon> GetOrpon(string data)
        {
            EntityResult<Entities.Orpon> result = new EntityResult<Entities.Orpon>();

            using (var client = new wsSearchAddrElByFullNamePortTypeClient(_nameEndpoint, _address))
            {
                try
                {
                    var add = new AddressElementNameDataAddressElementFullNameGroup[]
                     {
                        new AddressElementNameDataAddressElementFullNameGroup()
                        {
                         FullAddress = data
                        }
                     };

                    var r = client.SearchAddressElementByFullName(new Orpon.AddressElementNameData()
                    {
                        AddressElementFullNameList = add
                    });

                    if (r != null)
                    {
                        if (r.AddressElementResponseList.Length > 0)
                        {
                            var a = r.AddressElementResponseList[0];
                            result.Object = GetOrpon(a);
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
                    client.Close();
                }
            }

            return result;
        }

        private Entities.Orpon GetOrpon(AddressElementNameResponseAddressElementNameGroup a)
        {
            return new Entities.Orpon()
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

        public EntityResult<Entities.Orpon> GetOrpon(IEnumerable<string> data)
        {
            EntityResult<Entities.Orpon> result = new EntityResult<Entities.Orpon>();

            try
            {
                using (var client = new wsSearchAddrElByFullNamePortTypeClient(_nameEndpoint, _address))
                {
                    try
                    {
                        var address = data.Select(x =>
                        {
                            return new AddressElementNameDataAddressElementFullNameGroup()
                            {
                                FullAddress = x
                            };
                        }).ToArray();

                        var a = client.SearchAddressElementByFullName(new Orpon.AddressElementNameData()
                        {
                            AddressElementFullNameList = address
                        });

                        if (a != null)
                        {
                            result.Objects = a.AddressElementResponseList.Select(x =>
                            {
                                return GetOrpon(x);
                            });
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
                        if (client != null)
                        {
                            client.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
                result.Result = false;
            }

            return result;
        }

        public Task<EntityResult<Entities.Orpon>> GetOrponAsync(string data)
        {
            return Task.Run(() => GetOrpon(data));
        }

        public Task<EntityResult<Entities.Orpon>> GetOrponAsync(IEnumerable<string> data)
        {
            return Task.Run(() => GetOrpon(data));
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
                    using (var client = new Orpon.wsSearchAddrElByFullNamePortTypeClient(_nameEndpoint, _address))
                    {
                        try
                        {
                            client.Open();

                            result.Result = true;
                        }
                        catch (Exception ex)
                        {
                            result.Error = ex;
                            result.Result = false;
                        }
                        finally
                        {
                            if (client != null)
                            {
                                client.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                }

                return result;
            });
        }
    }
}