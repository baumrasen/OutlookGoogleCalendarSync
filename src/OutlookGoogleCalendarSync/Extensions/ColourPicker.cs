﻿using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OutlookGoogleCalendarSync.Extensions {
    #region Dropdown colour pickers
    public abstract class ColourPicker : ComboBox {
        public ColourPicker() {
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawFixed;
        }
    }

    public class OutlookColourPicker : ColourPicker {
        public OutlookColourPicker() {
            base.DrawItem += ColourPicker_DrawItem;
        }
        public void AddColourItems() {
            Items.Clear();
            AddCategoryColours();
            AddStandardColours();
        }

        /// <summary>
        /// Add all the available Outlook colours
        /// </summary>
        public void AddStandardColours() {
            foreach (KeyValuePair<OlCategoryColor, Color> colour in OutlookOgcs.Categories.Map.Colours) {
                Items.Add(new OutlookOgcs.Categories.ColourInfo(colour.Key, colour.Value));
            }
        }

        /// <summary>
        /// Add just the colours associated with categories
        /// </summary>
        public void AddCategoryColours() {
            Items.AddRange(OutlookOgcs.Calendar.Categories.DropdownItems().ToArray());
        }

        public void ColourPicker_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {
            ComboBox cbColour = sender as ComboBox;
            if (e == null || e.Index < 0 || e.Index >= cbColour.Items.Count)
                return;

            // Get the colour
            OutlookOgcs.Categories.ColourInfo colour = (OutlookOgcs.Categories.ColourInfo)Items[e.Index];
            ColourCombobox.DrawComboboxItemColour(cbColour, new SolidBrush(colour.Colour), colour.Text, e);
        }

        public new OutlookOgcs.Categories.ColourInfo SelectedItem {
            get { return (OutlookOgcs.Categories.ColourInfo)base.SelectedItem; }
            set { base.SelectedItem = value; }
        }
    }

    public class GoogleColourPicker : ColourPicker {
        public GoogleColourPicker() {
            DrawItem += ColourPicker_DrawItem;
        }

        /// <summary>
        /// Add all the available Google colours
        /// </summary>
        public void AddPaletteColours() {
            foreach (GoogleOgcs.EventColour.Palette palette in GoogleOgcs.Calendar.Instance.ColourPalette.ActivePalette) {
                Items.Add(palette);
            }
        }

        public void ColourPicker_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {
            ComboBox cbColour = sender as ComboBox;
            if (e == null || e.Index < 0 || e.Index >= cbColour.Items.Count)
                return;

            // Get the colour
            GoogleOgcs.EventColour.Palette colour = (GoogleOgcs.EventColour.Palette)Items[e.Index];
            ColourCombobox.DrawComboboxItemColour(cbColour, new SolidBrush(colour.RgbValue), colour.HexValue, e);
        }

        public new GoogleOgcs.EventColour.Palette SelectedItem {
            get { return (GoogleOgcs.EventColour.Palette)base.SelectedItem; }
            set { base.SelectedItem = value; }
        }
    }
    #endregion

    #region ColourComboboxColumns
    public class DataGridViewOutlookColourComboBoxColumn : DataGridViewColumn {
        public DataGridViewOutlookColourComboBoxColumn() : base(new DataGridViewOutlookColourComboBoxCell()) {
        }

        public override DataGridViewCell CellTemplate {
            get {
                return base.CellTemplate;
            }
            set {
                // Ensure that the cell used for the template is a DataGridViewOutlookColourComboBoxCell.
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewOutlookColourComboBoxCell))) {
                    throw new InvalidCastException("Must be a DataGridViewOutlookColourComboBoxCell");
                }
                base.CellTemplate = value;
            }
        }
    }

    public class DataGridViewGoogleColourComboBoxColumn : DataGridViewColumn {
        public DataGridViewGoogleColourComboBoxColumn() : base(new DataGridViewGoogleColourComboBoxCell()) {
        }

        public override DataGridViewCell CellTemplate {
            get {
                return base.CellTemplate;
            }
            set {
                // Ensure that the cell used for the template is a DataGridViewGoogleColourComboBoxCell.
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewGoogleColourComboBoxCell))) {
                    throw new InvalidCastException("Must be a DataGridViewGoogleColourComboBoxCell");
                }
                base.CellTemplate = value;
            }
        }
    }
    #endregion

    #region ColourComboBoxCells
    public class DataGridViewOutlookColourComboBoxCell : DataGridViewTextBoxCell {
        public DataGridViewOutlookColourComboBoxCell() : base() { }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle) {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            OutlookColourCombobox ctl = DataGridView.EditingControl as OutlookColourCombobox;
            if (this.RowIndex >= 0) {
                if (this.Value == null)
                    ctl.SelectedItem = (OutlookOgcs.Categories.ColourInfo)this.DefaultNewRowValue;
                else {
                    String currentText = this.Value.ToString();
                    if (ctl.Items.Count == 0) {
                        ctl.PopulateDropdownItems();
                    }
                    this.Value = currentText;
                    foreach (OutlookOgcs.Categories.ColourInfo ci in Forms.ColourMap.OutlookComboBox.Items) {
                        if (ci.Text == (String)this.Value) {
                            ctl.SelectedValue = ci;
                            break;
                        }
                    }
                }
            }
        }

        public override Type EditType {
            get {
                return typeof(OutlookColourCombobox);
            }
        }

        public override Type ValueType {
            get {
                return typeof(OutlookOgcs.Categories.ColourInfo);
            }
        }

        public override object DefaultNewRowValue {
            get {
                if (Forms.ColourMap.OutlookComboBox.Items.Count > 0)
                    return (Forms.ColourMap.OutlookComboBox.Items[1] as OutlookOgcs.Categories.ColourInfo).Text;
                else
                    return String.Empty;
            }
        }

        protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, System.Windows.Forms.DataGridViewElementStates elementState, object value, object formattedValue, string errorText, System.Windows.Forms.DataGridViewCellStyle cellStyle, System.Windows.Forms.DataGridViewAdvancedBorderStyle advancedBorderStyle, System.Windows.Forms.DataGridViewPaintParts paintParts) {
            //Paint inactive cells
            //base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            int indexItem = rowIndex;
            if (indexItem < 0)
                return;

            foreach (OutlookOgcs.Categories.ColourInfo ci in Forms.ColourMap.OutlookComboBox.Items) {
                if (ci.Text == this.Value.ToString()) {
                    Brush boxBrush = new SolidBrush(ci.Colour);
                    Brush textBrush = SystemBrushes.WindowText;
                    Extensions.ColourCombobox.DrawComboboxItemColour(true, boxBrush, textBrush, this.Value.ToString(), graphics, cellBounds);
                    break;
                }
            }
        }
    }

    public class DataGridViewGoogleColourComboBoxCell : DataGridViewTextBoxCell {
        public DataGridViewGoogleColourComboBoxCell() : base() { }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle) {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            GoogleColourCombobox ctl = DataGridView.EditingControl as GoogleColourCombobox;
            if (this.RowIndex >= 0) {
                if (this.Value == null)
                    ctl.SelectedItem = (GoogleOgcs.EventColour.Palette)this.DefaultNewRowValue;
                else {
                    String currentText = this.Value.ToString();
                    if (ctl.Items.Count == 0) {
                        ctl.PopulateDropdownItems();
                    }
                    this.Value = currentText;
                    foreach (GoogleOgcs.EventColour.Palette ci in Forms.ColourMap.GoogleComboBox.Items) {
                        if (ci.HexValue == (String)this.Value) {
                            ctl.SelectedValue = ci;
                            break;
                        }
                    }
                }
            }
        }

        public override Type EditType {
            get {
                return typeof(GoogleColourCombobox);
            }
        }

        public override Type ValueType {
            get {
                return typeof(GoogleOgcs.EventColour.Palette);
            }
        }

        public override object DefaultNewRowValue {
            get {
                if (Forms.ColourMap.GoogleComboBox.Items.Count > 0)
                    return (Forms.ColourMap.GoogleComboBox.Items[1] as GoogleOgcs.EventColour.Palette).HexValue;
                else
                    return String.Empty;
            }
        }

        protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, System.Windows.Forms.DataGridViewElementStates elementState, object value, object formattedValue, string errorText, System.Windows.Forms.DataGridViewCellStyle cellStyle, System.Windows.Forms.DataGridViewAdvancedBorderStyle advancedBorderStyle, System.Windows.Forms.DataGridViewPaintParts paintParts) {
            //Paint inactive cells
            //base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            
            if (rowIndex < 0)
                return;

            foreach (GoogleOgcs.EventColour.Palette ci in Forms.ColourMap.GoogleComboBox.Items) {
                if (ci.HexValue == this.Value.ToString()) {
                    Brush boxBrush = new SolidBrush(ci.RgbValue);
                    Brush textBrush = SystemBrushes.WindowText;
                    Extensions.ColourCombobox.DrawComboboxItemColour(true, boxBrush, textBrush, this.Value.ToString(), graphics, cellBounds);
                    break;
                }
            }
        }
    }
    #endregion

    #region ColourComboboxes
    public abstract class ColourCombobox : ComboBox {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public object EditingControlFormattedValue {
            get {
                return this.FormatString;
            }
            set {
                if (value is String) {
                    try {
                        this.FormatString = (string)value;
                    } catch {
                        this.FormatString = string.Empty;
                    }
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle) {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public DataGridView EditingControlDataGridView {
            get {
                return dataGridView;
            }
            set {
                dataGridView = value;
            }
        }

        public int EditingControlRowIndex {
            get {
                return rowIndex;
            }
            set {
                rowIndex = value;
            }
        }

        public bool EditingControlValueChanged {
            get {
                return valueChanged;
            }
            set {
                valueChanged = value;
            }
        }

        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey) {
            switch (key & Keys.KeyCode) {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public Cursor EditingPanelCursor {
            get {
                return base.Cursor;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll) {
        }

        public bool RepositionEditingControlOnValueChange {
            get {
                return false;
            }
        }

        protected void ComboboxColor_SelectedIndexChanged(object sender, EventArgs e) {
            if (dataGridView.SelectedCells != null && dataGridView.SelectedCells.Count > 0)
                dataGridView.SelectedCells[0].Value = this.Text;
        }

        public static void DrawComboboxItemColour(ComboBox cbColour, Brush boxColour, String itemDescription, DrawItemEventArgs e) {
            try {
                e.Graphics.FillRectangle(new SolidBrush(cbColour.BackColor), e.Bounds);
                e.DrawBackground();
                Boolean comboEnabled = cbColour.Enabled;

                // Write colour name
                Boolean highlighted = (e.State & DrawItemState.Selected) != DrawItemState.None;
                Brush brush = comboEnabled ? SystemBrushes.WindowText : SystemBrushes.InactiveCaptionText;
                if (highlighted)
                    brush = comboEnabled ? SystemBrushes.HighlightText : SystemBrushes.InactiveCaptionText;

                DrawComboboxItemColour(comboEnabled, boxColour, brush, itemDescription, e.Graphics, e.Bounds);

                // Draw the focus rectangle if appropriate
                if ((e.State & DrawItemState.NoFocusRect) == DrawItemState.None)
                    e.DrawFocusRectangle();
            } catch (System.Exception ex) {
                OGCSexception.Analyse(ex);
            }
        }

        public static void DrawComboboxItemColour(Boolean comboEnabled, Brush boxColour, Brush textColour, String itemDescription, Graphics graphics, Rectangle cellBounds) {
            try {
                // Draw colour box
                Rectangle colourbox = new Rectangle();
                colourbox.X = cellBounds.X + 2;
                colourbox.Y = cellBounds.Y + 2;
                colourbox.Height = cellBounds.Height - 5;
                colourbox.Width = 18;
                graphics.FillRectangle(boxColour, colourbox);
                graphics.DrawRectangle(comboEnabled ? SystemPens.WindowText : SystemPens.InactiveBorder, colourbox);

                int textX = cellBounds.X + colourbox.X + colourbox.Width + 2;

                graphics.DrawString(itemDescription, Control.DefaultFont, textColour,
                    /*cellBounds.X*/ +colourbox.X + colourbox.Width + 2,
                    cellBounds.Y + ((cellBounds.Height - Control.DefaultFont.Height) / 2));

            } catch (System.Exception ex) {
                OGCSexception.Analyse(ex);
            }
        }
    }

    public class OutlookColourCombobox : ColourCombobox, IDataGridViewEditingControl {
        public OutlookColourCombobox() {
            PopulateDropdownItems();

            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DrawItem += new DrawItemEventHandler(ComboboxColor_DrawItem);
            this.SelectedIndexChanged += new EventHandler(base.ComboboxColor_SelectedIndexChanged);
        }

        public void PopulateDropdownItems() {
            Dictionary<OutlookOgcs.Categories.ColourInfo, String> cbItems = new Dictionary<OutlookOgcs.Categories.ColourInfo, String>();
            foreach (OutlookOgcs.Categories.ColourInfo ci in Forms.ColourMap.OutlookComboBox.Items) {
                cbItems.Add(ci, ci.Text);
            }
            this.DataSource = new BindingSource(cbItems, null);
            this.DisplayMember = "Value";
            this.ValueMember = "Key";
        }

        void ComboboxColor_DrawItem(object sender, DrawItemEventArgs e) {
            ComboBox cbColour = sender as ComboBox;
            int indexItem = e.Index;
            if (indexItem < 0 || indexItem >= cbColour.Items.Count)
                return;

            KeyValuePair<OutlookOgcs.Categories.ColourInfo, String> kvp = (KeyValuePair< OutlookOgcs.Categories.ColourInfo, String>)cbColour.Items[indexItem];
            if (kvp.Key != null) {
                // Get the colour
                OlCategoryColor olColour = kvp.Key.OutlookCategory;
                Brush brush = new SolidBrush(OutlookOgcs.Categories.Map.RgbColour(olColour));

                DrawComboboxItemColour(cbColour, brush, kvp.Value, e);
            }
        }
    }

    public class GoogleColourCombobox : ColourCombobox, IDataGridViewEditingControl {
        public GoogleColourCombobox() {
            PopulateDropdownItems();

            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DrawItem += new DrawItemEventHandler(ComboboxColor_DrawItem);
            this.SelectedIndexChanged += new EventHandler(base.ComboboxColor_SelectedIndexChanged);
        }

        public void PopulateDropdownItems() {
            Dictionary <GoogleOgcs.EventColour.Palette, String> cbItems = new Dictionary<GoogleOgcs.EventColour.Palette, String>();
            foreach (GoogleOgcs.EventColour.Palette ci in Forms.ColourMap.GoogleComboBox.Items) {
                cbItems.Add(ci, ci.HexValue);
            }
            this.DataSource = new BindingSource(cbItems, null);
            this.DisplayMember = "Value";
            this.ValueMember = "Key";
        }

        void ComboboxColor_DrawItem(object sender, DrawItemEventArgs e) {
            ComboBox cbColour = sender as ComboBox;
            if (e.Index < 0 || e.Index >= cbColour.Items.Count)
                return;

            KeyValuePair<GoogleOgcs.EventColour.Palette, String> kvp = (KeyValuePair<GoogleOgcs.EventColour.Palette, String>)cbColour.Items[e.Index];
            if (kvp.Key != null) {
                // Get the colour
                Brush brush = new SolidBrush(kvp.Key.RgbValue);

                DrawComboboxItemColour(cbColour, brush, kvp.Value, e);
            }
        }
    }
    #endregion
}