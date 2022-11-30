namespace eventRegistration
{
    public interface IGuest
    {
        List<Guest> Get();
        //Guest GetId(int id);
        Guest AddEmployee(Guest guest);

    }
    public class guestRepo : IGuest
    {

        private List<Guest> guestes = new List<Guest>()
        {


        };
        public List<Guest> Get()
        {
            return guestes;
        }
        //public Guest GetId(int Id)
        //{
        //    return guestes.FirstOrDefault(f => f.Id == Id);
        //}

        public Guest AddEmployee(Guest guest)
        {
            guestes.Add(guest);
            return guest;

        }

    }
}
