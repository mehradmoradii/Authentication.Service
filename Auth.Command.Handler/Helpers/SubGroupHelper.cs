using Auth.Domain.AppGroups.Aggregate;
using Auth.Infrastracture.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Handler.Helpers
{
    public static class SubGroupHelper
    {
        
        public static async Task<List<AppGroup>> FindAllSubGroup(AppGroup mainGroup, List<AppGroup> subGroups)
        {
          
            subGroups.Add(mainGroup);
            if(mainGroup.Children.Count() >0)
            {
                foreach(var child in mainGroup.Children)
                {
                    await FindAllSubGroup(child, subGroups);
                }
            }
            return subGroups;
           

        }
    }
}
