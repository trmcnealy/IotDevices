using System.Collections.Generic;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public class AppliedOptionSet : OptionSet<AppliedOption>
    {
        public AppliedOptionSet()
        {
        }

        public AppliedOptionSet(IReadOnlyCollection<AppliedOption> options)
            : base(options)
        {
        }

        protected override bool HasAlias(AppliedOption option,
                                         string        alias)
        {
            return option.Option.HasAlias(alias);
        }

        protected override bool HasRawAlias(AppliedOption option,
                                            string        alias)
        {
            return option.Option.HasRawAlias(alias);
        }

        protected override IReadOnlyCollection<string> RawAliasesFor(AppliedOption option)
        {
            return option.Option.RawAliases;
        }
    }
}