namespace GitApp.Data
{
    public class BranchInfo
    {
        public string name { get; set; }
        public string RepositoryName { get; set; }
        public string BranchName { get; set; }
        public int CommitCount { get; set; }
        public string BuildStatus { get; set; }
    }
}
