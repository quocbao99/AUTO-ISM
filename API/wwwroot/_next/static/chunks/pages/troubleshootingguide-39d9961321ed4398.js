(self.webpackChunk_N_E=self.webpackChunk_N_E||[]).push([[111],{68891:function(e,n,l){(window.__NEXT_P=window.__NEXT_P||[]).push(["/troubleshootingguide",function(){return l(18449)}])},18449:function(e,n,l){"use strict";l.r(n),l.d(n,{default:function(){return Z}});var a=l(85893),i=l(73762),t=l(29630),o=l(30120),r=l(79072),s=l(91655),d=l(70918),c=l(91359),u=l(65828),h=l(87536),g=l(88767),x=l(13136),m=l(67294),p=l(51593),v=l(76429),j=l(2676),f=l(43170),C=l(37024);const b=e=>{let{}=e;const n=(0,C.q)(),[l,i]=(0,m.useState)("lists"),[b,Z]=(0,m.useState)(null),{reset:I,control:P,watch:F,handleSubmit:w,setValue:D}=((0,m.useCallback)((e=>{i(e)}),[]),(0,h.cI)({defaultValues:{pageSize:10,pageIndex:1,searchContent:""}})),O=F(),{data:y,isLoading:W,isFetching:L}=(0,g.useQuery)(["documentsAllQuery",4,O.brandID,O.eModelID,O.LineOffID,O.searchContent,O.pageIndex,O.pageSize],(()=>p.xl.getAll({...O,systemFileCategory:"4"})),{keepPreviousData:!0,refetchOnWindowFocus:!1}),N=((0,m.useMemo)((()=>(null===y||void 0===y?void 0:y.data.items)||[]),[null===y||void 0===y?void 0:y.data]),(0,j.Nn)()),_=(0,m.useMemo)((()=>{var e;return(null===N||void 0===N||null===(e=N.data)||void 0===e?void 0:e.items.map((e=>({value:"".concat(e.id),label:"".concat(e.name)}))))||[]}),[N]),S=(0,j.bP)(O.brandID),M=(0,m.useMemo)((()=>{var e;return(null===S||void 0===S||null===(e=S.data)||void 0===e?void 0:e.items.map((e=>({value:"".concat(e.id),label:"".concat(e.name)}))))||[]}),[S]),k=(0,j.AP)(O.eModelID),H=(0,m.useMemo)((()=>{var e;return(null===k||void 0===k||null===(e=k.data)||void 0===e?void 0:e.items.map((e=>({value:"".concat(e.id),label:"".concat(e.name)}))))||[]}),[k]);(0,m.useMemo)((()=>[{Header:"T\xean folder",accessor:"name",minWidth:250,Filter:()=>null},{Header:"H\xe3ng xe",accessor:"brandID",Cell:e=>{let{row:n}=e;return(0,a.jsx)(a.Fragment,{children:n.original.brandName})},Filter:e=>{let{column:n}=e;return(0,a.jsx)(f.fD,{column:n,options:_})},minWidth:250},{Header:"D\xf2ng xe",accessor:"eModelID",Cell:e=>{let{row:n}=e;return(0,a.jsx)(a.Fragment,{children:n.original.emodelName})},Filter:e=>{let{column:n}=e;return(0,a.jsx)(f.fD,{column:n,options:M})},minWidth:250},{Header:"\u0110\u1eddi xe",accessor:"lineOffName",minWidth:250},{Header:"S\u1ed1 folders con",accessor:"countChildFile",align:"right",Filter:()=>null}]),[N]);return(0,a.jsxs)("div",{children:[(0,a.jsxs)("div",{className:"mb-[36px] flex justify-between items-center",children:[(0,a.jsxs)("div",{children:[(0,a.jsx)(t.Z,{variant:"largeTitle",children:"Qu\u1ea3n l\xfd t\xe0i li\u1ec7u trouble shooting guide"}),(0,a.jsx)(t.Z,{children:"T\u1ea5t c\u1ea3 th\xf4ng tin li\xean quan \u0111\u1ebfn t\xe0i li\u1ec7u trouble shooting guide \u0111\u01b0\u1ee3c qu\u1ea3n l\xfd t\u1eeb \u0111\xe2y "})]}),(0,a.jsx)(v.HS,{title:"T\u1ea1o m\u1edbi th\u01b0\u01a1ng hi\u1ec7u",handleFunction:n.onOpen})]}),(0,a.jsxs)(o.Z,{className:"mb-[36px] flex justify-between items-center",children:[(0,a.jsxs)(o.Z,{className:"h-full flex flex-wrap gap-2",children:[(0,a.jsx)(o.Z,{sx:{minWidth:250},children:(0,a.jsx)(v.o,{isClearable:!0,options:_,getOptionLabel:e=>e.label,getOptionValue:e=>e.value,control:P,name:"brandID",placeholder:"L\u1ecdc h\xe3ng xe",onChange:()=>{D("eModelID",""),D("LineOffID","")}})}),(0,a.jsx)(o.Z,{sx:{minWidth:250},children:(0,a.jsx)(v.o,{isClearable:!0,options:M,getOptionLabel:e=>e.label,getOptionValue:e=>e.value,control:P,name:"eModelID",placeholder:"L\u1ecdc d\xf2ng xe",onChange:()=>{D("LineOffID","")}})}),(0,a.jsx)(o.Z,{sx:{minWidth:250},children:(0,a.jsx)(v.o,{isClearable:!0,options:H,getOptionLabel:e=>e.label,getOptionValue:e=>e.value,control:P,name:"LineOffID",placeholder:"L\u1ecdc theo n\u0103m"})})]}),(0,a.jsx)(o.Z,{})]}),(0,a.jsxs)(o.Z,{sx:{mt:"36px"},children:[W||L?(0,a.jsxs)(r.ZP,{container:!0,spacing:"27px",children:[(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(s.Z,{variant:"rectangular",height:100})},"WiringFolderCollapseCard-1"),(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(s.Z,{variant:"rectangular",height:100})},"WiringFolderCollapseCard-2"),(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(s.Z,{variant:"rectangular",height:100})},"WiringFolderCollapseCard-3"),(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(s.Z,{variant:"rectangular",height:100})},"WiringFolderCollapseCard-4")]}):(null===y||void 0===y?void 0:y.data.items.length)?(0,a.jsx)(o.Z,{children:(0,a.jsx)(r.ZP,{container:!0,spacing:"27px",children:y.data.items.map(((e,n)=>{const l=e.id==b;return(0,a.jsx)(r.ZP,{item:!0,sm:12,xl:6,children:(0,a.jsx)(x.GF,{handleClick:()=>{e.id==b?Z(null):Z(e.id)},data:e,isCollapsed:l,systemFileCategory:"4"})},"WiringFolderCollapseCard-".concat(n))}))})}):null,(0,a.jsx)(d.Z,{sx:{marginTop:"27px"},children:(0,a.jsx)(c.Z,{sx:{padding:"18px !important"},children:(0,a.jsx)(o.Z,{children:(0,a.jsx)(u.Z,{rowsPerPageOptions:[2,5,10,25,50],component:"div",count:(null===y||void 0===y?void 0:y.data.totalItem)||0,size:"small",page:(null===O||void 0===O?void 0:O.pageIndex)?O.pageIndex-1:0,onPageChange:(e,n)=>{D("pageIndex",n+1)},labelRowsPerPage:"S\u1ed1 folder m\u1ed7i trang",labelDisplayedRows:e=>{let{from:n,to:l,count:i,page:t}=e;return(0,a.jsx)("p",{children:"".concat(n,"\u2013").concat(l," c\u1ee7a ").concat(-1!==i?i:"nhi\u1ec1u h\u01a1n ".concat(l))})},rowsPerPage:O.pageSize||0,onRowsPerPageChange:e=>{D("pageSize",parseInt(e.target.value))}})})})})]}),(0,a.jsx)(x.$R,{isOpen:n.isOpen,onClose:n.onClose,systemFileCategory:"4"})]})};b.title="Qu\u1ea3n l\xfd Specifications",b.getLayout=e=>(0,a.jsx)(i.Z,{children:e});var Z=b}},function(e){e.O(0,[587,109,491,211,412,59,29,136,774,888,179],(function(){return n=68891,e(e.s=n);var n}));var n=e.O();_N_E=n}]);