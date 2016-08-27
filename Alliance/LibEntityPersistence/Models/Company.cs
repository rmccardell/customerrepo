using System;

namespace LibEntityPersistence.Models
{
    public class Company:PersistableEntity<Company>, IEquatable<Company>
    {
        public string Name { get; set; }
        public Address Address { get; set; }

        public Company()
        {
          
        }

        public Company(string name, Address address)
        {
            Name = name;
            Address = address;
      
        }

        public Company(Company company)
        {
            Name = company.Name;
            Address = company.Address;
            Id = company.Id;
        }

        public bool Equals(Company other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) 
                && Equals(Address, other.Address)
                 && Equals(Id, other.Id); 
        }

        public override Company Create(PersistableEntity<Company> entity)
        {
            return new Company((Company)entity);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Company) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Address != null ? Address.GetHashCode() : 0);
            }
        }
    }
}
