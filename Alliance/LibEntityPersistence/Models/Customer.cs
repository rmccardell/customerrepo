using System;

namespace LibEntityPersistence.Models
{
    public class Customer:PersistableEntity<Customer>, IEquatable<Customer>
    {
        public string FirstName { get;  set; }
        public string LastName { get; set; }
        public Address Address { get; set; }

        public Customer()
        {
            
        }

        public Customer(string firstname, string lastName, Address address)
        {
            FirstName = firstname;
            LastName = lastName;
            Address = address;
        }

        public Customer(Customer customer)
        {
            FirstName =  customer.FirstName;
            LastName =  customer.LastName;
            Address =  customer.Address;
            Id = customer.Id;
        }

        public bool Equals(Customer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) &&
                string.Equals(FirstName, other.FirstName) 
                && string.Equals(LastName, other.LastName)
                && Equals(Address, other.Address)
                && Equals(Id, other.Id);
        }

        public override Customer Create(PersistableEntity<Customer> entity)
        {
            return new Customer((Customer) entity);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Customer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Address != null ? Address.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
