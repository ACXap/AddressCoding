using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressCoding.BDRepository
{
    public class OrponDbRepository : IBDRepository
    {
        private const string ARGUMENT_CANNOT_NULL = "cannot be null";
        private const string TABLE_HOUSE = "ent_as_house";
        private const string TABLE_ADDRESS = "ent_as_addrobj";
        private const string TEMP_TABLE_ID = "session_infobig_id";
        private readonly Random _rnd = new Random();

        public bool CheckConnect(ConnectionSettingsDb set)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(set)))
            {
                con.Open();

                string existsTable = $"SELECT '{TABLE_HOUSE}'::regclass";
                using (NpgsqlCommand cmd = new NpgsqlCommand(existsTable, con))
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public List<EntityOrponAddress> GetCollectionAddress(IEnumerable<int> collectionId, ConnectionSettingsDb set)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(set)))
            {
                con.Open();
                var r = _rnd.Next(111, 999);

                string query = $"CREATE TEMP TABLE {TEMP_TABLE_ID}{r} (id integer)";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var writer = con.BeginBinaryImport($"COPY {TEMP_TABLE_ID}{r} (id) FROM STDIN BINARY"))
                {
                    foreach (var id in collectionId)
                    {
                        writer.StartRow();
                        writer.Write(id, NpgsqlDbType.Integer);
                    }
                    writer.Complete();
                }

                query = $"SELECT A.orponid, A.adr_adm_ter From {TEMP_TABLE_ID}{r} S, {TABLE_ADDRESS} A WHERE S.id=A.orponid and A.livestatus=1";


                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        var list = new List<EntityOrponAddress>();
                        while (reader.Read())
                        {
                            var item = new EntityOrponAddress()
                            {
                                GlobalId = reader.GetInt32(0),
                                Address = reader.GetString(1)
                            };

                            list.Add(item);
                        }

                        return list;
                    }
                }
            }
        }

        public List<EntityOrponAddress> GetCollectionHouse(IEnumerable<int> collectionId, ConnectionSettingsDb set)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(GetConnectionString(set)))
            {
                con.Open();
                var r = _rnd.Next(111, 999);

                string query = $"CREATE TEMP TABLE {TEMP_TABLE_ID}{r} (id integer)";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var writer = con.BeginBinaryImport($"COPY {TEMP_TABLE_ID}{r} (id) FROM STDIN BINARY"))
                {
                    foreach (var id in collectionId)
                    {
                        writer.StartRow();
                        writer.Write(id, NpgsqlDbType.Integer);
                    }
                    writer.Complete();
                }

                query = $"SELECT H.orponid, H.adr_adm_ter From {TEMP_TABLE_ID}{r} S, {TABLE_HOUSE} H WHERE S.id=H.orponid and H.livestatus=1";


                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        var list = new List<EntityOrponAddress>();
                        while (reader.Read())
                        {
                            var item = new EntityOrponAddress()
                            {
                                GlobalId = reader.GetInt32(0),
                                Address = reader.GetString(1)
                            };

                            list.Add(item);
                        }

                        return list;
                    }
                }
            }
        }

        /// <summary>
        /// Метод для формирования строки подключения
        /// </summary>
        /// <param name="conSettings">Свойства подключения</param>
        /// <returns>Строка подключения</returns>
        private string GetConnectionString(ConnectionSettingsDb conSettings)
        {
            return $"Server={conSettings.Server};" +
                $"Port={conSettings.Port};" +
                $"User Id={conSettings.Login};" +
                $"Password={conSettings.Password};" +
                $"Database={conSettings.BDName};" +
                "Timeout=900;" +
                "CommandTimeout=900;";
        }
    }
}