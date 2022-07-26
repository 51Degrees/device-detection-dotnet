using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Apple
{
    /// <summary>
    /// Property meta data class used by <see cref="AppleProfileEngine"/>
    /// </summary>
    public class ApplePropertyMetaData : AspectPropertyMetaData, IFiftyOneAspectPropertyMetaData
    {
        private bool disposedValue;

        private static readonly List<string> _dataTiers = new List<string>()
        {
            "N/A"
        };

        public ApplePropertyMetaData(
            IAspectEngine element, 
            string name, 
            Type type, 
            string category, 
            string description)
            : base(element, name, type, category, _dataTiers, true, description, null, false, null)
        {
        }

        public string Url => "N/A";

        public byte DisplayOrder => 0;

        public bool Mandatory => true;

        public bool List => false;

        public bool Obsolete => false;

        public bool Show => true;

        public bool ShowValues => false;

#pragma warning disable CA1721 // Property names should not match get methods
        // Addressing this warning would be a breaking change.
        // Also note that 'GetValues' has a slightly different purpose as
        // it returns an IEnumerable where 'Value' returns an IReadOnlyList.
        public IComponentMetaData Component => null;

        public IReadOnlyList<IValueMetaData> Values => null;

        public IValueMetaData DefaultValue => null;
#pragma warning restore CA1721 // Property names should not match get methods

        public int CompareTo(IFiftyOneAspectPropertyMetaData other)
        {
            if(other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(IFiftyOneAspectPropertyMetaData other)
        {
            if (other == null)
            {
                return false;
            }
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(string other)
        {
            return Name.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        public IComponentMetaData GetComponent()
        {
            return Component;
        }

        public IValueMetaData GetDefaultValue()
        {
            return DefaultValue;
        }

        public IValueMetaData GetValue(string valueName)
        {
            return null;
        }

        public IEnumerable<IValueMetaData> GetValues()
        {
            return Values;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ApplePropertyMetaData);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(ApplePropertyMetaData left, ApplePropertyMetaData right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(ApplePropertyMetaData left, ApplePropertyMetaData right)
        {
            return !(left == right);
        }

        public static bool operator <(ApplePropertyMetaData left, ApplePropertyMetaData right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(ApplePropertyMetaData left, ApplePropertyMetaData right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(ApplePropertyMetaData left, ApplePropertyMetaData right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(ApplePropertyMetaData left, ApplePropertyMetaData right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
    }
}
