using System;
using System.ComponentModel.DataAnnotations;

namespace Stack
{
    public abstract class Entity : IIdentifier, ITimestamps, ITimestampSetter, IValidatable, IEquatable<Entity>
    {
        [DbPrimary]
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public DateTime Updated { get; set; }
        public void SetTimestamps()
        {
            if (IsNew)
            {
                Created = DateTime.UtcNow;
            }
            Updated = DateTime.UtcNow;
        }

        [DbIgnore]
        public bool IsNew
        {
            get
            {
                return Id == 0;
            }
        }

        public void Validate()
        {
            Validator.Validate(this);
        }
        public ValidationError TryValidate()
        {
            try
            {
                Validate();
            }
            catch (ValidationException e)
            {
                return e.Error;
            }
            return ValidationError.Empty;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
        public override int GetHashCode()
        {
            return !IsNew ? Id.GetHashCode() : base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals((Entity)other);
        }
        public bool Equals(Entity other)
        {
            bool result = false;
            if (other != null)
            {
                Type currentType = GetType();
                Type otherType = other.GetType();
                if (currentType.Inherits(otherType) || otherType.Inherits(currentType))
                {
                    if (IsNew && other.IsNew)
                    {
                        result = ReferenceEquals(this, other);
                    }
                    else
                    {
                        result = Id.Equals(other.Id);
                    }
                }
            }
            return result;
        }
        public static bool operator==(Entity first, Entity second) 
        {
            bool result = false;
            if (ReferenceEquals(first, second))
            {
                result = true;
            }
            else if (!object.ReferenceEquals(first, null))
            {
                result = first.Equals(second);
            }
            else if (!object.ReferenceEquals(second, null))
            {
                result = second.Equals(first);
            }
            return result;
        }
        public static bool operator!=(Entity first, Entity second) 
        {
            return !(first == second);
        }
    }
}
