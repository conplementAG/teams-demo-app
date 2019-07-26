namespace Backend.Model
{
    public class TeamMember
    {
        public string Name { get; set; }
        public string Role { get; set; }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Role))
            {
                return -1;
            }

            return (Name + Role).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as TeamMember;
            if (other == null)
            {
                return false;
            }

            return string.Equals(Name, other.Name) && string.Equals(Role, other.Role);
        }
    }
}
