config.apiUrl = utils.getQueryString('apiUrl');
config.siteId = utils.getQueryString('siteId');

var $api = new utils.Api('/ss.form/templates');

var data = {
  pageConfig: null,
  pageAlert: null,
  pageType: 'loading',
  templateType: 'submit',
  templates: null,
  templateHtml: null,
};

var methods = {
  loadTemplates: function (templateType) {
    var $this = this;
    this.templateType = templateType;

    if (this.pageLoad) {
      utils.loading(true);
    }
    $api.get({
      siteId: config.siteId,
      templateType: this.templateType
    }, function (err, res) {
      utils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.templates = res.value;
      $this.pageLoad = true;
      $this.pageType = 'list';
    });
  },

  getPreviewUrl: function (template) {
    return template.templateUrl + '/' + template.id + '/index.html?siteId=' + config.siteId + '&formId=' + template.formId + '&apiUrl=' + encodeURIComponent(config.apiUrl);
  },

  btnGetTemplateHtmlClick: function (template) {
    this.templateHtml = '<stl:form>' + template.html + '</stl:form>';
    this.pageType = 'templateHtml';
  },

  btnCopyClick: function () {
    var copyTextarea = document.querySelector('.js-copytextarea');
    copyTextarea.focus();
    copyTextarea.select();

    try {
      document.execCommand('copy');
      alert({
        toast: true,
        type: 'success',
        title: "复制成功！",
        showConfirmButton: false,
        timer: 2000
      })
    } catch {}
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.loadTemplates('submit');
  }
});