using System.Collections.Generic;

namespace AddressCoding.BDRepository
{
    public interface IBDRepository
    {
        bool CheckConnect(ConnectionSettingsDb set);
        List<EntityOrponAddress> GetCollectionAddress(IEnumerable<int> collectionId, ConnectionSettingsDb set);
        List<EntityOrponAddress> GetCollectionHouse(IEnumerable<int> collectionId, ConnectionSettingsDb set);
    }
}