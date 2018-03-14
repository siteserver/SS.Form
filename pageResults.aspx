<%@ Page Language="C#" Inherits="SS.Form.Pages.PageResults" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body>

    <form runat="server">

      <ul class="nav nav-tabs">
        <li class="nav-item">
          <a class="nav-link" href="<%=PageLogsUrl%>">数据列表</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="<%=PageResultsUrl%>">数据统计</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="<%=PageFieldsUrl%>">字段管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="<%=PageSettingsUrl%>">选项设置</a>
        </li>
        <li class="nav-item">
          <asp:LinkButton id="LbTemplate" class="nav-link" onclick="LbTemplate_Click" runat="server">
            自定义模板
          </asp:LinkButton>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="<%=ReturnUrl%>">
            <i class="ion-ios-undo"></i>
            返回列表
          </a>
        </li>
      </ul>

      <!-- container start -->
      <div class="container">
        <div class="m-b-25"></div>

        <div class="row">
          <div class="col-sm-12">
            <div class="text-center card-box">
              <h3 class="text-success counter">
                <asp:Literal id="LtlCount" runat="server" />
              </h3>
              <p class="text-muted">
                <asp:Button class="btn btn-success" id="BtnExport" onclick="BtnExport_Click" Text="导 出" runat="server" />
              </p>
            </div>
          </div>
        </div>

        <div class="row">
          <asp:repeater id="RptFields" runat="server">
            <itemtemplate>
              <div class="col-sm-12">
                <div class="card-box">
                  <h4 class="text-dark  header-title m-t-0">
                    <asp:Literal id="ltlTitle" runat="server" />
                  </h4>
                  <p class="text-muted m-b-25 font-13"></p>

                  <asp:repeater id="rptItems" runat="server">
                    <itemtemplate>
                      <div class="card-box widget-user">
                        <div>
                          <h4 class="m-t-0 m-b-5">
                            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                          </h4>
                          <small class="text-warning">
                            <b>
                              <asp:Literal ID="ltlSummary" runat="server"></asp:Literal>
                            </b>
                          </small>
                          <div class="progress" style=" margin: 5px 0;">
                            <asp:Literal ID="ltlProgress" runat="server"></asp:Literal>
                          </div>
                        </div>
                      </div>
                    </itemtemplate>
                  </asp:repeater>

                  <div class="m-b-25"></div>

                </div>
              </div>
            </itemtemplate>
          </asp:repeater>

        </div>
      </div>
      <!-- container end -->

    </form>
  </body>

  </html>
  <script src="assets/plugin-utils/js/jquery.min.js"></script>
  <script src="assets/plugin-utils/js/bootstrap.min.js"></script>