var $getParameter = function (name) {
  var result = location.search.match(
    new RegExp('[?&]' + name + '=([^&]+)', 'i')
  );
  if (!result || result.length < 1) {
    return window.$config ? $config[name] : '';
  }
  return decodeURIComponent(result[1]);
};

var $api = axios.create({
  withCredentials: true
});

VeeValidate.Validator.localize('zh_CN');
Vue.use(VeeValidate);
VeeValidate.Validator.localize({
  zh_CN: {
    messages: {
      required: function (name) {
        return name + '不能为空';
      }
    }
  }
});
VeeValidate.Validator.extend('mobile', {
  getMessage: function () {
    return ' 请输入正确的手机号码';
  },
  validate: function (value, args) {
    return (
      value.length == 11 &&
      /^((13|14|15|16|17|18|19)[0-9]{1}\d{8})$/.test(value)
    );
  }
});

if (window.DatePicker) {
  Vue.component("date-picker", window.DatePicker.default);
}

(function() {
  var $vue = new Vue({
    el: "#form_submit",
    data: {
      apiUrl: $getParameter('apiUrl'),
      siteId: $getParameter('siteId'),
      formId: $getParameter('formId'),
      pageType: 'loading',
      fieldInfoList: [],
      title: '',
      description: '',
      isCaptcha: false,
      captcha: '',
      captchaUrl: null,
      captchaInValid: false,
      errorMessage: '',
      uploadUrl: null,
      files: []
    },
    components: {
      FilePond: vueFilePond.default(FilePondPluginFileValidateType, FilePondPluginImagePreview)
    },
    methods: {
      getUploadUrl: function(fieldInfo) {
        return this.uploadUrl + '&fieldId=' + fieldInfo.id;
      },
    
      imageUploaded: function(error, file) {
        if (!error) {
          var res = JSON.parse(file.serverId);
          var fieldInfo = _.find(this.fieldInfoList, function(o) { return o.id === res.fieldId; });
          if (fieldInfo.value){
			  fieldInfo.value += ',' + res.value;
		  }
		  else{
			  fieldInfo.value = res.value;
		  }
        }
      },
    
      imageRemoved: function(fieldInfo) {
        fieldInfo.value = [];
      },

      loadCaptcha: function () {
        this.captchaUrl = this.apiUrl + '/v1/captcha/FORM-CAPTCHA' + '?r=' + new Date().getTime();
      },

      submit: function () {
        var $this = this;

        var payload = {};
        for (var i = 0; i < this.fieldInfoList.length; i++) {
          var fieldInfo = this.fieldInfoList[i];
          payload[fieldInfo.title] = fieldInfo.value;
        }

        $this.pageType = 'loading';
        $api.post(this.apiUrl + '/ss.form/' + this.siteId + '/' + this.formId, payload)
          .then(function (res) {
            $this.pageType = 'success';
          })
          .catch(function (error) {
            $this.pageType = 'error';
            $this.errorMessage = error.response.data.message;
          });
      },

      checkCaptcha: function () {
        var $this = this;

        $api.post(this.apiUrl + '/v1/captcha/FORM-CAPTCHA/actions/check', {
            captcha: $this.captcha
          }).then(function (res) {
            $this.submit();
          })
          .catch(function (error) {
            $this.pageType = 'form';
            $this.captchaInValid = true;
          });
      },

      btnSubmitClick: function () {
        var $this = this;

        $this.captchaInValid = false;
        this.$validator.validate().then(function (result) {
          if (result) {
            if ($this.isCaptcha) {
              $this.checkCaptcha();
            } else {
              $this.submit();
            }
          }
        });
      }
    },
    created: function () {
      var $this = this;

      $api.post(this.apiUrl + '/ss.form/' + this.siteId + '/' + this.formId + '/actions/get')
        .then(function (res) {
          $this.fieldInfoList = res.data.value;
          for (var i = 0; i < $this.fieldInfoList.length; i++) {
            var fieldInfo = $this.fieldInfoList[i];
            if (fieldInfo.fieldType === 'CheckBox' || fieldInfo.fieldType === 'SelectMultiple') {
              fieldInfo.value = [];
            }
          }
          $this.title = res.data.title;
          $this.description = res.data.description;
          $this.isCaptcha = res.data.isCaptcha;
          $this.uploadUrl = $this.apiUrl + '/ss.form/' + $this.siteId + '/' + $this.formId + '/actions/upload?uploadToken=' + res.data.uploadToken;
          $this.loadCaptcha();
          $this.pageType = 'form';
        })
        .catch(function (error) {
          $this.pageType = 'error';
          $this.errorMessage = error.response.data.message;
        });

      FilePond.setOptions({
        server: {
          process: {
            withCredentials: true
          }
        }
      });
    }
  });
})();
