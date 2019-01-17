using System.Collections.Generic;
using System.Linq;
using SharpArch.Domain.DomainModel;
using System;

namespace Bitsie.Shop.Domain
{
    public class Invoice : Entity
    {

        #region Constructor

        public Invoice()
        {
            
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Merhcant
        /// </summary>
        public virtual User Merchant{ get; set; }

        /// <summary>
        /// Invoice NUmber
        /// </summary>
        public virtual string InvoiceNumber { get; set; }

        /// <summary>
        /// Invoice GUID
        /// </summary>
        public virtual string InvoiceGuid { get; set; }

        /// <summary>
        /// Customer account for this invoice
        /// </summary>
        public virtual User Customer { get; set; }

        /// <summary>
        /// Invoice Amount
        /// </summary>
        public virtual decimal USDAmount { get; set; }

        /// <summary>
        /// Invoice Status
        /// </summary>
        public virtual InvoiceStatus Status { get; set; }

        /// <summary>
        /// Invoice Send Date
        /// </summary>
        public virtual DateTime? SendDate { get; set; }
        
        /// <summary>
        /// Invoice Due Date
        /// </summary>
        public virtual DateTime DueDate { get; set; }

        /// <summary>
        /// Invoice Description
        /// </summary>
        public virtual string InvoiceDescription { get; set; }

        /// <summary>
        /// Invoice Items
        /// </summary>
        public virtual IList<InvoiceItem> InvoiceItem { get; set; }

        #endregion

    }
}