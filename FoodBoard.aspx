<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FoodBoard.aspx.cs" Inherits="FoodBoard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="board-intro">
        <div>
            <h2>זמין עכשיו</h2>
            <p class="chat-item-context">מטעמי פרטיות מוצגת כאן רק כתובת חלקית ומרחק משוער. הכתובת המדויקת נמסרת ע"י המפרסם/ת דרך הצ'אט.</p>
            <asp:PlaceHolder ID="phLocationPrivacyNote" runat="server" Visible="false">
                <p class="chat-item-context">המרחקים המסומנים "מדויק" מחושבים לפי המיקום (GPS) ששיתפת בהרשמה - הוא לא מוצג לאף אחד, רק משמש לחישוב מרחק.</p>
            </asp:PlaceHolder>
        </div>
    </div>

    <div class="board-filters">
        <div>
            <label>עיר איסוף</label>
            <asp:DropDownList ID="ddlCity" runat="server"></asp:DropDownList>
        </div>
        <div>
            <label>קטגוריה</label>
            <asp:DropDownList ID="ddlCategory" runat="server"></asp:DropDownList>
        </div>
        <div>
            <label>פג תוקף תוך</label>
            <asp:DropDownList ID="ddlExpiry" runat="server">
                <asp:ListItem Value="" Text="הכל" Selected="True" />
                <asp:ListItem Value="3" Text="3 ימים" />
                <asp:ListItem Value="7" Text="7 ימים" />
                <asp:ListItem Value="14" Text="14 יום" />
                <asp:ListItem Value="30" Text="30 יום" />
            </asp:DropDownList>
        </div>
        <asp:Button ID="btnSearch" runat="server" Text="סנן" OnClick="btnSearch_Click" />
        <asp:Button ID="btnShowAll" runat="server" Text="הצג הכל" OnClick="btnShowAll_Click" />
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="chat-item-context"></asp:Label>

    <asp:Repeater ID="rptItems" runat="server" OnItemCommand="rptItems_ItemCommand">
        <HeaderTemplate>
            <div class="card-grid">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="item-card">
                <asp:PlaceHolder runat="server" Visible='<%# ((FoodItemView)Container.DataItem).HasPhoto %>'>
                    <div class="item-card-photo">
                        <img src='<%# Eval("PhotoUrl") %>' alt="" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" Visible='<%# !((FoodItemView)Container.DataItem).HasPhoto %>'>
                    <div class="item-card-photo-blocked">
                        <span>אין תמונה למוצר זה</span>
                    </div>
                </asp:PlaceHolder>
                <div class="item-card-body">
                    <div class="item-card-title-row">
                        <span class="item-card-name"><%# Eval("ItemName") %></span>
                        <span class='<%# "item-card-badge" + (((FoodItemView)Container.DataItem).IsUrgent ? " urgent" : "") %>'><%# Eval("ExpiryLabel") %></span>
                    </div>
                    <div class="item-card-meta"><%# Eval("MetaLine") %></div>
                    <div class="item-card-footer">
                        <div class="avatar"><%# Eval("Initials") %></div>
                        <a class="item-card-owner" href='<%# "Profile.aspx?id=" + Eval("UserId") %>' style="text-decoration:none; color:inherit;"><%# Eval("UserName") %></a>
                        <div class="item-card-actions">
                            <asp:HyperLink runat="server" NavigateUrl='<%# "EditItem.aspx?id=" + Eval("ItemId") %>'
                                Text="עריכה" Visible='<%# CanEdit(Eval("UserId")) %>' />
                            <asp:HyperLink runat="server" NavigateUrl='<%# "Chat.aspx?with=" + Eval("UserId") + "&item=" + Eval("ItemId") %>'
                                Text="הודעה" Visible='<%# CanMessage(Eval("UserId")) %>' />
                            <asp:Button runat="server" Text="מחיקה" CommandName="DeleteItem" CssClass="delete-btn"
                                CommandArgument='<%# Eval("ItemId") %>'
                                Visible='<%# CanEdit(Eval("UserId")) %>'
                                OnClientClick="return confirm('למחוק את המוצר?');" />
                            <asp:Button runat="server" Text="דווח על תמונה" CommandName="ReportPhoto" CssClass="report-btn"
                                CommandArgument='<%# Eval("ItemId") %>'
                                Visible='<%# CanReportPhoto(Eval("UserId"), Eval("PhotoUrl")) %>'
                                OnClientClick="return confirm('לדווח על התמונה כלא הולמת/מזויפת?');" />
                        </div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>

    <asp:Label ID="lblEmpty" runat="server" Text="לא נמצאו מוצרים." Visible="false" CssClass="chat-item-context"></asp:Label>
</asp:Content>
