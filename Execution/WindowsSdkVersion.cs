using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Core.Execution
{
    public readonly struct WindowsSdkVersion : IEquatable<WindowsSdkVersion>
    {
        public readonly ushort Major;

        public readonly ushort Minor;

        public readonly ushort Build;

        public readonly ushort Revision;

        public readonly string Version;

        private static ushort GetMajor(string version)
        {
            string[] splitString = version.Split('.');

            if (splitString.Length >= 1)
            {
                return ushort.Parse(splitString[0]);
            }

            throw new IndexOutOfRangeException();
        }

        private static ushort GetMinor(string version)
        {
            string[] splitString = version.Split('.');

            if (splitString.Length >= 2)
            {
                return ushort.Parse(splitString[1]);
            }

            throw new IndexOutOfRangeException();
        }

        private static ushort GetBuild(string version)
        {
            string[] splitString = version.Split('.');

            if (splitString.Length >= 3)
            {
                return ushort.Parse(splitString[2]);
            }

            throw new IndexOutOfRangeException();
        }

        private static ushort GetRevision(string version)
        {
            string[] splitString = version.Split('.');

            if (splitString.Length >= 4)
            {
                return ushort.Parse(splitString[3]);
            }

            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private float GetAsFloat()
        {
            return Major + (Minor / 100) + (Build / 100 / 100) + (Revision / 100 / 100 / 100);
        }

        public WindowsSdkVersion(ushort major, ushort minor, ushort build, ushort revision)
        {
            Major    = major;
            Minor    = minor;
            Build    = build;
            Revision = revision;
            Version  = major.ToString() + '.' + minor + '.' + build + '.' + revision;
        }

        public WindowsSdkVersion(string version)
        {
            Major    = GetMajor(version);
            Minor    = GetMinor(version);
            Build    = GetBuild(version);
            Revision = GetRevision(version);
            Version  = version;
        }

        public static WindowsSdkVersion MakeVersion(ushort major, ushort minor, ushort build, ushort revision)
        {
            return new WindowsSdkVersion(major, minor, build, revision);
        }

        #region Equality members

        public bool Equals(WindowsSdkVersion other)
        {
            return Major == other.Major && Minor == other.Minor && Build == other.Build && Revision == other.Revision;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is WindowsSdkVersion other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Major.GetHashCode();
                hashCode = (hashCode * 397) ^ Minor.GetHashCode();
                hashCode = (hashCode * 397) ^ Build.GetHashCode();
                hashCode = (hashCode * 397) ^ Revision.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        public static bool operator ==(in WindowsSdkVersion lhs, in WindowsSdkVersion rhs)
        {
            if (lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Build == rhs.Build && lhs.Revision == rhs.Revision)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(in WindowsSdkVersion lhs, in WindowsSdkVersion rhs)
        {
            if (lhs.Major != rhs.Major && lhs.Minor != rhs.Minor && lhs.Build != rhs.Build && lhs.Revision != rhs.Revision)
            {
                return true;
            }

            return false;
        }

        public static bool operator <(in WindowsSdkVersion lhs, in WindowsSdkVersion rhs)
        {
            if (lhs.Major < rhs.Major)
            {
                return true;
            }

            if (lhs.Minor < rhs.Minor)
            {
                return true;
            }

            if (lhs.Build < rhs.Build)
            {
                return true;
            }

            if (lhs.Revision < rhs.Revision)
            {
                return true;
            }

            return false;
        }

        public static bool operator >(in WindowsSdkVersion lhs, in WindowsSdkVersion rhs)
        {
            if (lhs.Major > rhs.Major)
            {
                return true;
            }

            if (lhs.Minor > rhs.Minor)
            {
                return true;
            }

            if (lhs.Build > rhs.Build)
            {
                return true;
            }

            if (lhs.Revision > rhs.Revision)
            {
                return true;
            }

            return false;
        }

        public static bool operator <=(in WindowsSdkVersion lhs, in WindowsSdkVersion rhs)
        {
            if (lhs.Major <= rhs.Major)
            {
                return true;
            }

            if (lhs.Minor <= rhs.Minor)
            {
                return true;
            }

            if (lhs.Build <= rhs.Build)
            {
                return true;
            }

            if (lhs.Revision <= rhs.Revision)
            {
                return true;
            }

            return false;
        }

        public static bool operator >=(in WindowsSdkVersion lhs, in WindowsSdkVersion rhs)
        {
            if (lhs.Major >= rhs.Major)
            {
                return true;
            }

            if (lhs.Minor >= rhs.Minor)
            {
                return true;
            }

            if (lhs.Build >= rhs.Build)
            {
                return true;
            }

            if (lhs.Revision >= rhs.Revision)
            {
                return true;
            }

            return false;
        }

        public static bool operator ==(in WindowsSdkVersion lhs, in uint rhs)
        {
            if (lhs.Major == rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(in WindowsSdkVersion lhs, in uint rhs)
        {
            if (lhs.Major != rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator <(in WindowsSdkVersion lhs, in uint rhs)
        {
            if (lhs.Major < rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator >(in WindowsSdkVersion lhs, in uint rhs)
        {
            if (lhs.Major > rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator <=(in WindowsSdkVersion lhs, in uint rhs)
        {
            if (lhs.Major <= rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator >=(in WindowsSdkVersion lhs, in uint rhs)
        {
            if (lhs.Major >= rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator ==(in WindowsSdkVersion lhs, in float rhs)
        {
            if (lhs.GetAsFloat() == rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(in WindowsSdkVersion lhs, in float rhs)
        {
            if (lhs.GetAsFloat() != rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator <(in WindowsSdkVersion lhs, in float rhs)
        {
            if (lhs.GetAsFloat() < rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator >(in WindowsSdkVersion lhs, in float rhs)
        {
            if (lhs.GetAsFloat() > rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator <=(in WindowsSdkVersion lhs, in float rhs)
        {
            if (lhs.GetAsFloat() <= rhs)
            {
                return true;
            }

            return false;
        }

        public static bool operator >=(in WindowsSdkVersion lhs, in float rhs)
        {
            if (lhs.GetAsFloat() >= rhs)
            {
                return true;
            }

            return false;
        }
    }
}