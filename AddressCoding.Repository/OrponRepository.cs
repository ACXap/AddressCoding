﻿using AddressCoding.Entities;
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

        public EntityResult<bool> GetOrpon(EntityOrpon data)
        {
            EntityResult<bool> result = new EntityResult<bool>();

            using (var client = new wsSearchAddrElByFullNamePortType2Client(_nameEndpoint, _address))
            {
                try
                {
                    data.Status = StatusType.OrponingNow;

                    var add = new AddressElementNameDataAddressElementFullNameGroup[]
                     {
                        new AddressElementNameDataAddressElementFullNameGroup()
                        {
                         FullAddress = data.Address
                        }
                     };

                    var r = client.SearchAddressElementByFullName(new AddressElementNameData()
                    {
                        AddressElementFullNameList = add
                    });

                    if (r != null)
                    {
                        if (r.AddressElementResponseList2.Length > 0)
                        {
                            var a = r.AddressElementResponseList2[0];
                            data.Orpon = GetOrpon(a);
                            data.Status = StatusType.OK;
                            result.Result = true;
                        }
                        else
                        {
                            data.Error = "Error AddressElementResponseList.Count = 0";
                            data.Status = StatusType.Error;
                            result.Result = false;
                        }
                    }
                    else
                    {
                        result.Result = false;
                        data.Status = StatusType.Error;
                    }
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                    result.Result = false;
                    data.Status = StatusType.Error;
                }
                finally
                {
                    client.Close();
                }
            }

            return result;
        }

        private Entities.Orpon GetOrpon(AddressElementNameResponse2AddressElementNameGroup2 a)
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
                SystemCode = a.SystemCode,
                Latitude = a.Latitude,
                LocalityGlobalId = a.LocalityGlobalId,
                LocationDescription = a.LocationDescription,
                Longitude = a.Longitude,
                StreetGlobalId = a.StreetGlobalId,
                UnparsedParts = a.UnparsedParts,
                HouseGlobalId = a.HouseGlobalId
            };
        }

        public EntityResult<bool> GetOrpon(IEnumerable<EntityOrpon> data, bool canCheckSinglObj)
        {
            EntityResult<bool> result = new EntityResult<bool>();

            try
            {
                using (var client = new wsSearchAddrElByFullNamePortType2Client(_nameEndpoint, _address))
                {
                    try
                    {
                        if (canCheckSinglObj)
                        {
                            foreach (var d in data)
                            {
                                d.Status = StatusType.OrponingNow;

                                var add = new AddressElementNameDataAddressElementFullNameGroup[]
                                 {
                                        new AddressElementNameDataAddressElementFullNameGroup()
                                        {
                                         FullAddress = d.Address
                                        }
                                 };

                                var r = client.SearchAddressElementByFullName(new AddressElementNameData()
                                {
                                    AddressElementFullNameList = add
                                });

                                if (r != null)
                                {
                                    if (r.AddressElementResponseList2.Length > 0)
                                    {
                                        var a = r.AddressElementResponseList2[0];
                                        d.Orpon = GetOrpon(a);
                                        d.Status = StatusType.OK;
                                        //result.Result = true;
                                    }
                                    else
                                    {
                                        d.Error = "Error AddressElementResponseList.Count = 0";
                                        d.Status = StatusType.Error;
                                        //result.Result = false;
                                    }
                                }
                                else
                                {
                                    //result.Result = false;
                                    d.Status = StatusType.Error;
                                }

                                d.DateTimeOrponing = DateTime.Now;
                            }
                        }
                        else
                        {
                            var address = data.Select(x =>
                            {
                                x.Status = StatusType.OrponingNow;
                                return new AddressElementNameDataAddressElementFullNameGroup()
                                {
                                    FullAddress = x.Address
                                };
                            }).ToArray();

                            client.Open();

                            var a = client.SearchAddressElementByFullName(new AddressElementNameData()
                            {
                                AddressElementFullNameList = address
                            });

                            if (a != null)
                            {
                                var indexRow = 0;
                                foreach (var k in a.AddressElementResponseList2)
                                {
                                    data.ElementAt(indexRow).Orpon = GetOrpon(k);
                                    data.ElementAt(indexRow).Status = StatusType.OK;
                                    data.ElementAt(indexRow).DateTimeOrponing = DateTime.Now;
                                    indexRow++;
                                }

                                //result.Objects = a.AddressElementResponseList2.Select(x =>
                                //{
                                //    return GetOrpon(x);
                                //});
                            }
                            else
                            {
                                result.Result = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        foreach (var i in data)
                        {
                            i.Error = ex.Message;
                            i.DateTimeOrponing = DateTime.Now;
                            i.Status = StatusType.Error;
                        }
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

        public Task<EntityResult<bool>> GetOrponAsync(EntityOrpon data)
        {
            return Task.Run(() => GetOrpon(data));
        }

        public Task<EntityResult<bool>> GetOrponAsync(IEnumerable<EntityOrpon> data, bool canCheckSinglObj)
        {
            return Task.Run(() => GetOrpon(data, canCheckSinglObj));
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
                    using (var client = new Orpon.wsSearchAddrElByFullNamePortType2Client(_nameEndpoint, _address))
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