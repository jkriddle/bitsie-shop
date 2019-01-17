using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace Bitsie.Shop.Infrastructure
{
    [Serializable]
    public class FreeTextExpression : AbstractCriterion
    {
        private readonly string _propertyName;
        private readonly object _value;
        private readonly IProjection _projection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainsExpression"/> class.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <param name="value">The value.</param>
        public FreeTextExpression(IProjection projection, object value)
        {
            _projection = projection;
            _value = value;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="ContainsExpression" />
        /// class for a named Property and its value.
        /// </summary>
        /// <param name="propertyName">The name of the Property in the class.</param>
        /// <param name="value">The value for the Property.</param>
        public FreeTextExpression(string propertyName, object value)
        {
            _propertyName = propertyName;
            _value = value;
        }

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
        {
            var sqlBuilder = new SqlStringBuilder();
            SqlString[] columnNames = null;

            if (!_propertyName.Contains("*"))
            {
                columnNames = CriterionUtil.GetColumnNames(_propertyName, _projection, criteriaQuery, criteria, enabledFilters);

                if (columnNames.Length != 1)
                {
                    throw new HibernateException("Contains may only be used with single-column properties");
                }
            } else
            {
                columnNames = new SqlString[]
                                  {
                                      new SqlString("*")
                                  };
            }

            sqlBuilder.Add("freetext(")
              .Add(columnNames[0])
              .Add(",");

            sqlBuilder.Add(criteriaQuery.NewQueryParameter(GetParameterTypedValue(criteria, criteriaQuery)).Single());
            sqlBuilder.Add(")");

            return sqlBuilder.ToSqlString();
        }

        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            var typedValues = new List<TypedValue>();

            if (_projection != null)
            {
                typedValues.AddRange(_projection.GetTypedValues(criteria, criteriaQuery));
            }
            typedValues.Add(GetParameterTypedValue(criteria, criteriaQuery));

            return typedValues.ToArray();
        }

        public TypedValue GetParameterTypedValue(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            var matchValue = _value.ToString();
            if (_projection != null)
            {
                return CriterionUtil.GetTypedValues(criteriaQuery, criteria, _projection, null, matchValue).Single();
            }
            return new TypedValue(NHibernateUtil.String, _value.ToString(), EntityMode.Poco);
        }

        public override IProjection[] GetProjections()
        {
            if (_projection != null)
            {
                return new[] { _projection };
            }
            return null;
        }

        public override string ToString()
        {
            return " contains(" + (_projection ?? (object)_propertyName) + "," + _value + ")";
        }
    }
}