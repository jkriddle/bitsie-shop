using SharpArch.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public class Queue : Entity
    {

        public Queue()
        {
            QueueDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Request action to take
        /// </summary>
        public virtual QueueAction Action { get; set; }

        /// <summary>
        /// Request data (JSON objects, parameters, etc)
        /// </summary>
        public virtual string Input { get; set; }

        /// <summary>
        /// Request status
        /// </summary>
        public virtual QueueStatus Status { get; set; }

        /// <summary>
        /// URL associated with the request
        /// </summary>
        public virtual string Url { get; set; }

        /// <summary>
        /// Unique ID/hash
        /// </summary>
        public virtual string Guid { get; set; }

        /// <summary>
        /// Queue date
        /// </summary>
        public virtual DateTime QueueDate { get; set; }
    }
}
