using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace ISIProject.Utils
{
    public static class LinkedDropdownHelper
    {
        #region Methods

        public static MvcHtmlString LinkedDropdownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string parent, IEnumerable<LinkedSelectListItem> selectList, bool removedefault = false)
        {
            string propertyName = ((MemberExpression)expression.Body).Member.Name;

            TagBuilder select = new TagBuilder("select");
            select.Attributes.Add("id", propertyName);
            select.Attributes.Add("name", propertyName);
            //select.Attributes.Add("class", "linked-dropdown");
            select.Attributes.Add("class", parent);

            foreach (var item in selectList)
            {
                if (removedefault && item.Value == "-1")
                {
                    //skip default
                }
                else
                {
                    TagBuilder option = new TagBuilder("option");
                    option.InnerHtml = item.Text;
                    option.Attributes.Add("value", item.Value);
                    option.Attributes.Add("class", item.LinkValue);

                    if (item.Selected)
                    {
                        option.Attributes.Add("selected", "selected");
                    }
                    select.InnerHtml += option.ToString(TagRenderMode.Normal);
                }
            }
            //below code was changed by abdurrauf to support jquery chains
            string script = @"<script type='text/javascript'>$(document).bind('ready', function(){$(function(){
                    $('#" + propertyName + "').chained('#" + parent + "');});});</script>";

            return MvcHtmlString.Create(script + select.ToString(TagRenderMode.Normal));
        }

        #endregion Methods
    }

    public class LinkedSelectList : IEnumerable<LinkedSelectListItem>
    {
        #region Constructors

        public LinkedSelectList(IEnumerable items, string dataValueField, string dataTextField, string dataLinkedValueField, IEnumerable selectedValues)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            Items = items;
            DataValueField = dataValueField;
            DataTextField = dataTextField;
            DataLinkedValueField = dataLinkedValueField;
            SelectedValues = selectedValues;
        }

        #endregion Constructors

        #region Properties

        public string DataLinkedValueField
        {
            get;
            private set;
        }

        public string DataTextField
        {
            get;
            private set;
        }

        public string DataValueField
        {
            get;
            private set;
        }

        public IEnumerable Items
        {
            get;
            private set;
        }

        public IEnumerable SelectedValues
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public virtual IEnumerator<LinkedSelectListItem> GetEnumerator()
        {
            return GetListItems().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal IList<LinkedSelectListItem> GetListItems()
        {
            return GetListItemsWithValueField();
        }

        private static string Eval(object container, string expression)
        {
            object value = container;
            if (!String.IsNullOrEmpty(expression))
            {
                value = DataBinder.Eval(container, expression);
            }
            return Convert.ToString(value, CultureInfo.CurrentCulture);
        }

        private IList<LinkedSelectListItem> GetListItemsWithValueField()
        {
            HashSet<string> selectedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (SelectedValues != null)
            {
                selectedValues.UnionWith(from object value in SelectedValues select Convert.ToString(value, CultureInfo.CurrentCulture));
            }

            var listItems = from object item in Items
                            let value = Eval(item, DataValueField)
                            select new LinkedSelectListItem
                            {
                                Value = value,
                                Text = Eval(item, DataTextField),
                                LinkValue = Eval(item, DataLinkedValueField),
                                Selected = selectedValues.Contains(value)
                            };
            return listItems.ToList();
        }

        #endregion Methods
    }

    public class LinkedSelectListItem
    {
        #region Properties

        public string LinkValue
        {
            get;
            set;
        }

        public bool Selected
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        #endregion Properties
    }
}