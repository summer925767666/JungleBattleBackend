namespace GameServer.Model
{
    public class Result
    {
        public int Id { get;  set; }
        public int UserId { get; private set; }
        public int TotalCount { get; set; }
        public int WinCount { get; set; }

        public Result(int id, int userId, int totalCount, int winCount)
        {
            Id = id;
            UserId = userId;
            TotalCount = totalCount;
            WinCount = winCount;
        }
    }
}