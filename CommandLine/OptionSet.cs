using System.Collections.Generic;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public class OptionSet : OptionSet<Option>
    {
        protected override bool HasAlias(Option option,
                                         string alias)
        {
            return option.HasAlias(alias);
        }

        protected override bool HasRawAlias(Option option,
                                            string alias)
        {
            return option.HasRawAlias(alias);
        }

        protected override IReadOnlyCollection<string> RawAliasesFor(Option option)
        {
            return option.RawAliases;
        }
    }
}