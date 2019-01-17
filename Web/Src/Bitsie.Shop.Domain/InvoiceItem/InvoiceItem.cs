using System.Collections.Generic;
using System.Linq;
using SharpArch.Domain.DomainModel;
using System;

namespace Bitsie.Shop.Domain
{
    public class InvoiceItem : Entity
    {

        #region Constructor

        public InvoiceItem()
        {
            
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Invoice
        /// </summary>
        public virtual Invoice Invoice{ get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }
        
        /// <summary>
        /// Item Amount
        /// </summary>
        public virtual decimal UsdAmount { get; set; }

        /// <summary>
        /// Item Quantity
        /// </summary>
        public virtual int Quantity { get; set; }

        /// <summary>
        /// Item Order Position
        /// </summary>
        public virtual int Position { get; set; }

        #endregion


    }
}
