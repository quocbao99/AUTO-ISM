(self.webpackChunk_N_E=self.webpackChunk_N_E||[]).push([[197],{85713:function(e,t,i){(window.__NEXT_P=window.__NEXT_P||[]).push(["/tableapi",function(){return i(79578)}])},79578:function(e,t,i){"use strict";i.r(t),i.d(t,{default:function(){return f}});var n=i(85893),a=i(62097),l=i(29630),s=i(26993),o=i(88767),r=i(73762),c=i(76429),d=i(53073),u=i(51593),h=i(87536),g=i(67294);function f(){(0,a.Z)();const{register:e,handleSubmit:t,watch:i,formState:r,reset:f,control:v,setValue:m}=(0,h.cI)({defaultValues:{pageSize:25,pageIndex:1}}),p=i(),{data:w,isLoading:x,isFetching:b}=(0,o.useQuery)(["fake",p],(()=>u.bY.getAll({...p})),{onError:e=>{},onSuccess:e=>{},refetchOnWindowFocus:!1,keepPreviousData:!0}),_=(0,g.useMemo)((()=>(null===w||void 0===w?void 0:w.data.items)||[]),[null===w||void 0===w?void 0:w.data]),j=(0,g.useMemo)((()=>[{Header:"T\xean d\xe2n t\u1ed9c",accessor:"name",sticky:"left"},{Header:"Ghi ch\xfa",accessor:"description"}]),[]);return(0,n.jsxs)("div",{children:[(0,n.jsxs)("div",{className:"mb-[36px] flex justify-between items-center",children:[(0,n.jsxs)("div",{children:[(0,n.jsx)(l.Z,{variant:"largeTitle",children:"Users Management"}),(0,n.jsx)(l.Z,{children:"All aspects related to the app users can be managed from this page"})]}),(0,n.jsx)(d.HS,{title:"th\xeam",handleFunction:()=>{}})]}),(0,n.jsx)(c.ZD,{viewProps:{title:"DATA FAKE",darkTitle:!0},data:_,totalRows:(null===w||void 0===w?void 0:w.data.totalItem)||0,totalPage:(null===w||void 0===w?void 0:w.data.totalPage)||0,onDoubleClick:e=>{},onEditClick:e=>{},multiSelectAction:e=>(0,n.jsx)("div",{children:(0,n.jsx)(s.Z,{onClick:()=>{},color:"error",children:"Delete"})}),tableOptions:{columns:j},isToggleHiddenColumn:!0,tablePlugins:["globalFilter","pagination","rowSelect"],searchState:p,setNewState:e=>{f(e)},isLoading:x})]})}f.getLayout=e=>(0,n.jsx)(r.Z,{children:e})}},function(e){e.O(0,[587,109,59,29,774,888,179],(function(){return t=85713,e(e.s=t);var t}));var t=e.O();_N_E=t}]);