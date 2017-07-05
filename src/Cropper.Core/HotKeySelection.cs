#region License Information

/**********************************************************************************
Shared Source License for Cropper
Copyright 9/07/2004 Brian Scott
http://blogs.geekdojo.net/brian

This license governs use of the accompanying software ('Software'), and your
use of the Software constitutes acceptance of this license.

You may use the Software for any commercial or noncommercial purpose,
including distributing derivative works.

In return, we simply require that you agree:
1. Not to remove any copyright or other notices from the Software. 
2. That if you distribute the Software in source code form you do so only
   under this license (i.e. you must include a complete copy of this license
   with your distribution), and if you distribute the Software solely in
   object form you only do so under a license that complies with this
   license.
3. That the Software comes "as is", with no warranties. None whatsoever.
   This means no express, implied or statutory warranty, including without
   limitation, warranties of merchantability or fitness for a particular
   purpose or any warranty of title or non-infringement. Also, you must pass
   this disclaimer on whenever you distribute the Software or derivative
   works.
4. That no contributor to the Software will be liable for any of those types
   of damages known as indirect, special, consequential, or incidental
   related to the Software or this license, to the maximum extent the law
   permits, no matter what legal theory it�s based on. Also, you must pass
   this limitation of liability on whenever you distribute the Software or
   derivative works.
5. That if you sue anyone over patents that you think may apply to the
   Software for a person's use of the Software, your license to the Software
   ends automatically.
6. That the patent rights, if any, granted in this license only apply to the
   Software, not to any derivative works you make.
7. That the Software is subject to U.S. export jurisdiction at the time it
   is licensed to you, and it may be subject to additional export or import
   laws in other places.  You agree to comply with all such laws and
   regulations that may apply to the Software after delivery of the software
   to you.
8. That if you are an agency of the U.S. Government, (i) Software provided
   pursuant to a solicitation issued on or after December 1, 1995, is
   provided with the commercial license rights set forth in this license,
   and (ii) Software provided pursuant to a solicitation issued prior to
   December 1, 1995, is provided with �Restricted Rights� as set forth in
   FAR, 48 C.F.R. 52.227-14 (June 1987) or DFAR, 48 C.F.R. 252.227-7013
   (Oct 1988), as applicable.
9. That your rights under this License end automatically if you breach it in
   any way.
10.That all rights not expressly granted to you in this license are reserved.

**********************************************************************************/

#endregion

#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fusion8.Cropper.Core
{
    public partial class HotKeySelection : UserControl
    {
        public event EventHandler<HotKeyRegistrationEventArgs> HotKeyRegister;

        public HotKeySelection()
        {
            InitializeComponent();

            SelectFirst();
        }

        private void SelectFirst()
        {
            if (shortcutList.Items.Count <= 0)
                return;
            shortcutList.Focus();
            shortcutList.Items[0].Selected = true;
        }

        public void AddRange(IEnumerable<HotKeyData> hotKeyData)
        {
            if (hotKeyData == null)
                return;

            foreach (HotKeyData keyAction in hotKeyData)
            {
                ListViewItem item = new ListViewItem(new[] { keyAction.Name, ShortcutTextBox.GetShorcutText(keyAction.KeyData) });
                item.Tag = keyAction;
                item.ForeColor = keyAction.Hide ? SystemColors.GrayText : SystemColors.ControlText;

                if (!string.IsNullOrEmpty(keyAction.Group))
                {
                    ListViewGroup listGroup = null;
                    foreach (ListViewGroup g in Groups)
                    {
                        if (!string.Equals(keyAction.Group, g.Header))
                            continue;
                        listGroup = g;
                        break;
                    }

                    if (listGroup == null)
                        Groups.Add(listGroup = new ListViewGroup(keyAction.Group));

                    item.Group = listGroup;
                }
                Items.Add(item);
            }
            SelectFirst();
        }

        private void HandleShortcutAssignClick(object sender, EventArgs e)
        {
            AddShortcut();
            shortcutKeyNames.Enabled = true;
            shortcutKeyNames.Visible = true;
        }

        private void AddShortcut()
        {
            if (shortcutList.SelectedItems.Count == 0)
                return;

            ListViewItem item = shortcutList.SelectedItems[0];

            if (item.SubItems.Count == 1)
                item.SubItems.Add(shortcut.Text);
            else
                item.SubItems[1].Text = shortcut.Text;


            HotKeyData hotKeyData = ((HotKeyData)item.Tag);

            OnHotKeyRegister(new HotKeyRegistrationEventArgs(hotKeyData.Id, item.Text, shortcut.KeyData, hotKeyData.Action, hotKeyData.KeyData, hotKeyData.Global));

            hotKeyData.KeyData = shortcut.KeyData;
            shortcutKeyNames.Text = shortcut.Text;
        }

        private void HandleShortcutListSelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = shortcutList.SelectedItems;
            if (items.Count == 0 || items[0] == null || items[0].Tag == null)
                return;

            HotKeyData hotKeyData = ((HotKeyData)items[0].Tag);

            shortcut.Mode = hotKeyData.Global ? ShortcutMode.Global : ShortcutMode.Local;

            shortcutKeyNames.Text = null;
            shortcutKeyNames.Visible = true;
            shortcut.KeyData = hotKeyData.KeyData;
            shortcutKeyNames.Text = shortcut.Text;
        }

        private void HandleShortcutPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            AddShortcut();
        }

        private void HandleShortcutEnter(object sender, EventArgs e)
        {
            Debug.WriteLine("Shortcut Enter");

            shortcut.Clear();
            shortcutKeyNames.Visible = false;
        }

        private void HandleShortcutKeyNamesEnter(object sender, EventArgs e)
        {
            Debug.WriteLine("ShortcutKeyNames Enter");
            shortcut.Focus();
        }

        private void OnHotKeyRegister(HotKeyRegistrationEventArgs e)
        {
            EventHandler<HotKeyRegistrationEventArgs> handler = HotKeyRegister;
            if (handler != null)
                handler(this, e);
        }
    }
}