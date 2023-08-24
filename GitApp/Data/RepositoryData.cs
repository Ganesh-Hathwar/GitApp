namespace GitApp.Data
{
    public class RepositoryData
    {
        public string name { get; set; }
        public Owner owner { get; set; }
        public int BranchCount { get; set; }
        public string TeamName { get; set; }
    }
}
