using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace UsersLibrary
{
    public class AccountDatabase : ObservableCollection<Account>
    {
        public Account? FindAccount(Account user)
        {
            foreach (var account in this)
            {
                if (account.Email == user.Email)
                {
                    return account;
                }
            }
            return null;
        }

        public bool AddAccount(Account user)
        {
            if (FindAccount(user) != null) return false;
            Add(user);
            return true;
        }

        public void SaveToJson(FileInfo jsonFile)
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(jsonFile.FullName, json);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Nemôžeš uložiť do súboru: " + e.Message);
                throw;
            }
        }

        public static AccountDatabase? LoadFromJson()
        {
            try
            {
                StreamReader streamReader = new("accounts.json");
                AccountDatabase accounts = [];
                var json = streamReader.ReadToEnd();
                var accountsList = JsonConvert.DeserializeObject<List<Account>>(json)!;
            
                foreach (var account in accountsList)
                {
                    accounts.Add(account);
                }
            
                streamReader.Close();
                return accounts;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Nemôžeš načítať zo súboru: " + e.Message);
                throw;
            }
        }
    }
}
