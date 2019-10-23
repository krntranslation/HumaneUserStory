using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }

        internal static void RemoveEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new NotImplementedException("employee doesn't exist!");
            }
            else
            {
                db.Employees.DeleteOnSubmit(employee);
                db.SubmitChanges();
            }
        }

        //// TODO Items: ////

        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            Console.WriteLine("What would you like to do?\n1)Create new Employee\n2)Retrieve existing employee\n3)Update employee\n4)Delete employee");
            int input = Convert.ToInt32(Console.ReadLine());
            switch (input)
            {
                case 1:
                    UserEmployee userEmployee = new UserEmployee();
                    userEmployee.CreateNewEmployee();
                    break;

                case 2:
                    RetrieveEmployeeUser();
                    break;

                case 3:
                    UserEmployee userEmployee1 = new UserEmployee();
                    userEmployee1.UpdateEmployeeInfo();
                    break;

                case 4:
                    RemoveEmployee(employee);
                    break;

                default:
                    UserInterface.DisplayUserOptions("Input not accepted please try again");
                    break;
            }
        }

        private static void RetrieveEmployeeUser()
        {
            throw new NotImplementedException();
        }

        // DONE: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();

            if (animal == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                db.Animals.InsertOnSubmit(animal);
                db.SubmitChanges();
            }
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();

            if (animalFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return animalFromDb;
            }
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animal = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();
            if (animal == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                foreach (KeyValuePair<int, string> update in updates)
                {
                    if (update.Key == 1 || update.Key == 2 || update.Key == 3 || update.Key == 4 || update.Key == 5 || update.Key == 6 || update.Key == 7)
                    {
                        animal.Category.Name = update.Value;
                    }
                }

                db.SubmitChanges();
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            if (animal == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                db.Animals.DeleteOnSubmit(animal);
                db.SubmitChanges();
            }
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates, string input) // parameter(s)?
        {
            IQueryable<Animal> animalList = db.Animals;
            foreach (KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        animalList = animalList.Where(a => a.CategoryId == GetCategoryId(update.Value));
                        break;
                    case 2:
                        animalList = animalList.Where(a => a.Name == update.Value);
                        break;
                    case 3:
                        animalList = animalList.Where(a => a.Age == Convert.ToInt32(update.Value));
                        break;
                    case 4:
                        animalList = animalList.Where(a => a.Demeanor == update.Value);
                        break;
                    case 5:
                        animalList = animalList.Where(a => a.KidFriendly == Convert.ToBoolean(update.Value));
                        break;
                    case 6:
                        animalList = animalList.Where(a => a.PetFriendly == Convert.ToBoolean(update.Value));
                        break;
                    case 7:
                        animalList = animalList.Where(a => a.Weight == Convert.ToInt32(update.Value));
                        break;
                    case 8:
                        animalList = animalList.Where(a => a.AnimalId == Convert.ToInt32(update.Value));
                        break;
                }

                
            }return animalList;

        }

        internal static int GetCategoryId(string categoryName)
        {
            if (categoryName == null)
            {
                throw new System.ArgumentException("Not a valid category");
            }
            else
            {
                Category category = db.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
                return category.CategoryId;
            }
        }

        internal static Room GetRoom(int animalId)
        {
            Room animalFromDb = db.Rooms.Where(a => a.AnimalId == animalId).FirstOrDefault();

            if (animalFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return animalFromDb;
            }
        }

        internal static int GetDietPlanId(string dietPlanName)
        {
            DietPlan animalFromDb = db.DietPlans.Where(d => d.Name == dietPlanName).FirstOrDefault();

            if (animalFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return animalFromDb.DietPlanId;
            }
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)//specify from database that client is adopting an animal
        {
            if (animal == null)
            {
                throw new System.ArgumentException("Animal ID or animal is not valid");
            }
            else
            {
                Adoption adoption = new Adoption();
                adoption.ClientId = client.ClientId;
                adoption.AnimalId = animal.AnimalId;
                adoption.ApprovalStatus = "Pending";
                adoption.AdoptionFee = 75;
                adoption.PaymentCollected = false;
                db.Adoptions.InsertOnSubmit(adoption);
                db.SubmitChanges();
            }
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            var pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "Pending");
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (adoption == null)
            {
                throw new System.ArgumentException("Adoption not valid");
            }
            else
            {
                if (isAdopted == true)
                {
                    adoption.ApprovalStatus = "Approved";
                }
                else
                {
                    adoption.ApprovalStatus = "Not Approved";
                }
            }
        }
        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoption = db.Adoptions.Where(a => a.AnimalId == a.AnimalId && a.ClientId == a.ClientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoption);
            db.SubmitChanges();
        }


        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var shots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return shots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot newShot = new AnimalShot();
            var animalVacination = db.Shots.Where(s => s.Name == shotName).FirstOrDefault();
            if (animalVacination == null)
            {
                Shot shot = new Shot() { Name = shotName };
                db.Shots.InsertOnSubmit(shot);
                db.SubmitChanges();
                animalVacination = db.Shots.Where(s => s.Name == shotName).FirstOrDefault();
            }
            newShot.AnimalId = animal.AnimalId;
            newShot.DateReceived = DateTime.Now;
            newShot.ShotId = animalVacination.ShotId;

            db.AnimalShots.InsertOnSubmit(newShot);
            db.SubmitChanges();
        }
    }
}