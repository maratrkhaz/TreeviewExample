using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeViewExample.Dto
{
    public class UnitTreeDto
    {
        public UnitTreeDto(string id, string type, string parent, string name)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Type = type;
            Parent = parent;
            Text = name;
        }

        public string Id { get; }
        public string Type { get; }
        public string Parent { get; }
        public string Text { get; }
    }
}
