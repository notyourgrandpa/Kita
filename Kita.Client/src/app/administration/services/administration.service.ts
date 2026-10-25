import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../../environments/environment";
import { ImportDataResult } from "../models/import-data-result";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class AdministrationService{

    constructor(
        private http: HttpClient
    ){
    }

    getUrl(url: string): string{
        return environment.baseUrl + url;
    }

    importData(file: File): Observable<ImportDataResult>{
        const url = this.getUrl("api/seed/import");

        const formData = new FormData();
        formData.append('file', file);
        return this.http.post<ImportDataResult>(url, formData);
    }
}