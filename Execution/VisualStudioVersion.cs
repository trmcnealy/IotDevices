using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Core.Execution
{
    public readonly struct VisualStudioVersion : IEquatable<VisualStudioVersion>
    {
        public readonly ushort Major;

        public readonly ushort Minor;

        public readonly ushort Build;

        public readonly string Version;

        private static ushort GetMajor(string version)
        {
            string[] splitString = version.Split('.');

            if(splitString.Length >= 1)
            {
                return ushort.Parse(splitString[0]);
            }

            throw new IndexOutOfRangeException();
        }

        private static ushort GetMinor(string version)
        {
            string[] splitString = version.Split('.');

            if(splitString.Length >= 2)
            {
                return ushort.Parse(splitString[1]);
            }

            throw new IndexOutOfRangeException();
        }

        private static ushort GetBuild(string version)
        {
            string[] splitString = version.Split('.');

            if(splitString.Length >= 3)
            {
                return ushort.Parse(splitString[2]);
            }

            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private float GetAsFloat()
        {
            return Major + Minor / 100 + Build / 100 / 100;
        }

        public VisualStudioVersion(ushort major, ushort minor, ushort build)
        {
            Major   = major;
            Minor   = minor;
            Build   = build;
            Version = major.ToString() + '.' + minor + '.' + build;
        }

        public VisualStudioVersion(string version)
        {
            Major   = GetMajor(version);
            Minor   = GetMinor(version);
            Build   = GetBuild(version);
            Version = version;
        }

        public static VisualStudioVersion MakeVersion(ushort major, ushort minor, ushort build)
        {
            return new VisualStudioVersion(major, minor, build);
        }

        #region Equality members

        public bool Equals(VisualStudioVersion other)
        {
            return Major == other.Major && Minor == other.Minor && Build == other.Build;
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is VisualStudioVersion other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Major.GetHashCode();
                hashCode = (hashCode * 397) ^ Minor.GetHashCode();
                hashCode = (hashCode * 397) ^ Build.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        public static bool operator ==(in VisualStudioVersion lhs, in VisualStudioVersion rhs)
        {
            if(lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Build == rhs.Build)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(in VisualStudioVersion lhs, in VisualStudioVersion rhs)
        {
            if(lhs.Major != rhs.Major && lhs.Minor != rhs.Minor && lhs.Build != rhs.Build)
            {
                return true;
            }

            return false;
        }

        public static bool operator <(in VisualStudioVersion lhs, in VisualStudioVersion rhs)
        {
            if(lhs.Major < rhs.Major)
            {
                return true;
            }

            if(lhs.Minor < rhs.Minor)
            {
                return true;
            }

            if(lhs.Build < rhs.Build)
            {
                return true;
            }

            return false;
        }

        public static bool operator >(in VisualStudioVersion lhs, in VisualStudioVersion rhs)
        {
            if(lhs.Major > rhs.Major)
            {
                return true;
            }

            if(lhs.Minor > rhs.Minor)
            {
                return true;
            }

            if(lhs.Build > rhs.Build)
            {
                return true;
            }

            return false;
        }

        public static bool operator <=(in VisualStudioVersion lhs, in VisualStudioVersion rhs)
        {
            if(lhs.Major <= rhs.Major)
            {
                return true;
            }

            if(lhs.Minor <= rhs.Minor)
            {
                return true;
            }

            if(lhs.Build <= rhs.Build)
            {
                return true;
            }

            return false;
        }

        public static bool operator >=(in VisualStudioVersion lhs, in VisualStudioVersion rhs)
        {
            if(lhs.Major >= rhs.Major)
            {
                return true;
            }

            if(lhs.Minor >= rhs.Minor)
            {
                return true;
            }

            if(lhs.Build >= rhs.Build)
            {
                return true;
            }

            return false;
        }

        public static bool operator ==(in VisualStudioVersion lhs, in uint rhs)
        {
            if(lhs.Major == rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(in VisualStudioVersion lhs, in uint rhs)
        {
            if(lhs.Major != rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator <(in VisualStudioVersion lhs, in uint rhs)
        {
            if(lhs.Major < rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator >(in VisualStudioVersion lhs, in uint rhs)
        {
            if(lhs.Major > rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator <=(in VisualStudioVersion lhs, in uint rhs)
        {
            if(lhs.Major <= rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator >=(in VisualStudioVersion lhs, in uint rhs)
        {
            if(lhs.Major >= rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator ==(in VisualStudioVersion lhs, in float rhs)
        {
            if(lhs.GetAsFloat() == rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(in VisualStudioVersion lhs, in float rhs)
        {
            if(lhs.GetAsFloat() != rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator <(in VisualStudioVersion lhs, in float rhs)
        {
            if(lhs.GetAsFloat() < rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator >(in VisualStudioVersion lhs, in float rhs)
        {
            if(lhs.GetAsFloat() > rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator <=(in VisualStudioVersion lhs, in float rhs)
        {
            if(lhs.GetAsFloat() <= rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator >=(in VisualStudioVersion lhs, in float rhs)
        {
            if(lhs.GetAsFloat() >= rhs)
            {
                return true;
            }

            return false;
        }
    }
}