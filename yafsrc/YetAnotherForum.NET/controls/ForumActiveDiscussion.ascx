<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumActiveDiscussion.ascx.cs"
	Inherits="YAF.Controls.ForumActiveDiscussion" %>
<table border="0" class="content" cellspacing="1" cellpadding="0" width="100%">
	<tr>
		<td class="header1" colspan="2">
			<asp:ImageButton runat="server" ID="expandActiveDiscussions" BorderWidth="0" ImageAlign="Baseline"
				OnClick="expandActiveDiscussions_Click" />&nbsp;&nbsp;<YAF:LocalizedLabel ID="ActiveDiscussionHeader"
					runat="server" LocalizedTag="ACTIVE_DISCUSSIONS" />
		</td>
	</tr>
	<asp:PlaceHolder runat="server" ID="ActiveDiscussionPlaceHolder">
		<tr>
			<td class="header2" colspan="2">
				<YAF:LocalizedLabel ID="LatestPostsHeader" runat="server" LocalizedTag="LATEST_POSTS" />
			</td>
		</tr>
		<asp:Repeater runat="server" ID="LatestPosts" OnItemDataBound="LatestPosts_ItemDataBound">
			<ItemTemplate>
				<tr>
					<td class="post" valign="top">
						&nbsp;<b><asp:HyperLink ID="TextMessageLink" runat="server" /></b>
						<YAF:LocalizedLabel ID="ByLabel" runat="server" LocalizedTag="BY" />
						<YAF:UserLink ID="LastUserLink" runat="server" />
						(<asp:HyperLink ID="ForumLink" runat="server" />)
					</td>
					<td class="post" valign="top" style="width:20em;text-align:right;">
						<asp:Label ID="LastPostedDateLabel" runat="server" />
						<asp:HyperLink ID="ImageMessageLink" runat="server">
							<YAF:ThemeImage ID="LastPostedImage" runat="server" Style="border: 0" />
						</asp:HyperLink>							
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</asp:PlaceHolder>
</table>
