var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/ss.form/pages/templateHtml/",
  params: {
    siteId: utils.getQueryString('siteId')
  },
  withCredentials: true
});

var data = {
  siteId: utils.getQueryString('siteId'),
  type: utils.getQueryString('type'),
  apiUrl: utils.getQueryString('apiUrl'),
  name: utils.getQueryString('name'),
  pageLoad: false,
  pageConfig: null,
  pageAlert: null,
  templateInfo: null,
  templateHtml: null,
  isSystem: false,
  editor: null
};

var methods = {
  apiGetTemplateInfo: function () {
    var $this = this;
    
    utils.loading(true);
    $api.get('', {
      params: {
        name: this.name
      }
    }).then(function (response) {
      var res = response.data;

      $this.templateInfo = res.value;
      $this.templateHtml = res.templateHtml;
      $this.isSystem = res.isSystem;

      setTimeout(function () {
        require.config({ paths: { 'vs': '../assets/lib/monaco-editor/min/vs' }});
        require(['vs/editor/editor.main'], function() {
            $this.editor = monaco.editor.create(document.getElementById('container'), {
                value: $this.templateHtml,
                language: 'html'
            });
            $this.editor.focus();
        });

        $('#container').css({
          height: $(document).height() - 200
        });
      }, 100);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
      $this.pageLoad = true;
    });
  },

  getEditorContent: function() {
    return this.editor.getModel().getValue();
  },

  btnSubmitClick: function () {
    if (this.isSystem) {
      var url = 'templatesLayerEdit.html?siteId=' + this.siteId + '&apiUrl=' + encodeURIComponent(this.apiUrl) + '&type=' + this.type + '&name=' + this.name + '&isSystem=true';

      utils.openLayer({
        title: '克隆模板',
        url: url
      });
      return;
    }

    var $this = this;
    this.templateHtml = this.getEditorContent();
    utils.loading(true);
    $api.post('', {
      name: this.name,
      templateHtml: this.templateHtml
    }).then(function (response) {
      var res = response.data;

      swal({
        toast: true,
        type: 'success',
        title: "模板编辑成功！",
        showConfirmButton: false,
        timer: 2000
      }).then(function () {
        $this.btnNavClick($this.type);
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnNavClick: function(type) {
    utils.loading(true);
    var url = 'templates.html?siteId=' + this.siteId + '&apiUrl=' + encodeURIComponent(this.apiUrl) + '&formId=' + this.formId + '&type=' + type;
    location.href = url;
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGetTemplateInfo();
  }
});