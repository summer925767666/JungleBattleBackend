namespace GameServer.Model
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Pwd { get; private set; }

        public User(int id, string name, string pwd)
        {
            Id = id;
            Name = name;
            Pwd = pwd;
        }
    }
}
