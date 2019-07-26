using System.Collections.Generic;

namespace Backend.Model
{
    public class Team
    {
        public List<TeamMember> Members { get; set; } = new List<TeamMember>();
        public string TeamName { get; set; }

        public override int GetHashCode()
        {
            if(string.IsNullOrEmpty(TeamName))
            {
                return -1;
            }

            return TeamName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Team;
            if(other == null)
            {
                return false;
            }

            return string.Equals(TeamName, other.TeamName);
        }
    }
}