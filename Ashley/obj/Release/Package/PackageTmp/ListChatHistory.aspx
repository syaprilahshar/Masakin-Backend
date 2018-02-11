<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ListChatHistory.aspx.cs" Inherits="Masakin.ListChatHistory" %>
<%@ MasterType virtualpath="~/Default.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager runat="server" />
    <div class="row">
        <div class="col-md-12 col-sm-12 col-xs-12">
            <div class="dashboard_graph">

                <div class="row x_title">
                    <div class="col-md-12">
                        <h3>Chat Histories of <asp:Label runat="server" ID="lblUsername"/></h3>
                    </div>
                </div>

                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div style="width: 100%;">
                        <div id="canvas_dahs" class="demo-placeholder" style="width: 100%; height: 100%;">
                            <div class="row">
                                <%--<div class="col-md-2 col-sm-2 col-xs-4">
                                    <asp:DropDownList runat="server" ID="ddlDS" CssClass="form-control" >
                                        <asp:ListItem Text="Semua Diskusi" Value="0" />
                                        <asp:ListItem Text="Belum Dibalas" Value="1" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-2 col-sm-2 col-xs-4">
                                    <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-success" Text="CARI" OnClick="btnSearch_Click" />
                                </div>
                                <div class="col-md-2 col-sm-2 col-xs-4">
                                    
                                </div>
                            </div>
                            <hr />--%>
                            <asp:GridView CssClass="table table-striped jambo_table bulk_action" ID="GridView1" runat="server" AutoGenerateColumns="false" DataKeyNames="RoomID"
                                EnableViewState="true"
                                OnRowDataBound="GridView1_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="No.">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                            <asp:HiddenField runat="server" ID="hfDiscussionID" Value='<%# Eval("DiscussionID") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Username" HeaderText="Username" />
                                    <asp:BoundField DataField="Fullname" HeaderText="Fullname" />
                                    <asp:BoundField DataField="Email" HeaderText="Email" />
                                    <asp:BoundField DataField="Rate" HeaderText="Rate" />
                                    <asp:BoundField DataField="Review" HeaderText="Review" />
                                    <%--<asp:TemplateField HeaderText="Balas Cepat" ItemStyle-Width="200" ItemStyle-HorizontalAlign="Center" >
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtReply" CssClass="form-control" runat="server" placeholder="jawaban..." />
                                            <asp:Button ID="btnReply" CssClass="form-control" runat="server" Text="Balas" CausesValidation="false" OnClick="btnReply_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Detail Chat" ItemStyle-Width="75" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Button ID="btnViewChat" CssClass="btn btn-default" runat="server" Text="view" CausesValidation="false" OnClick="btnViewChat_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Hapus" ItemStyle-Width="75" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Button ID="btnDelete" CssClass="btn btn-danger" runat="server" Text="Hapus" CausesValidation="false" OnClick="btnDelete_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
                <div class="clearfix"></div>
            </div>
        </div>

    </div>
    <br />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript">
        $(window).load(function () {
            if (window.isColorbox) {
                $.colorbox({ href: "BalasanDiskusi.aspx", iframe: true, width: "80%", height: "80%" });
            }
        });
        function OpenCBox() {
            $.colorbox({ href: "BalasanDiskusi.aspx", iframe: true, width: "80%", height: "80%" });
            }
        
    </script>
</asp:Content>
