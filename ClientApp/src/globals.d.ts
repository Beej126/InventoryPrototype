//this satisfies typescript checking of import .svg under .tsx in src/components folder
//i'm wondering why it wasn't cranky when same thing was done under just src/app.tsx
declare module "*.svg" {
  export const content: any;
  //export default content;
}

//full global, we can just refer to this directly in any .ts/.tsx file
//by convention these will be initialized in App.tsx
//https://stackoverflow.com/questions/38906359/create-a-global-variable-in-typescript/43523944#43523944
interface Window {
  baseUrl: string;
}
declare var baseUrl: string;

//defined in app.tsx for now
interface String {
  trimStart: (trimChar: ' ' | ',' | '/') => string;
  trimEnd: (trimChar: ' ' | ',' | '/') => string;
}
