import { Component, OnInit, ElementRef, Input, OnDestroy } from '@angular/core';
import { ModalService } from '../_services/modal.service';
import { Conversation } from '../_models/conversation';

@Component({
  selector: 'app-add-addresse-modal',
  template:
        `<div class="app-modal">
            <div class="app-modal-body">
                <ng-content></ng-content>
            </div>
        </div>
        <div class="app-modal-background"></div>`
})
export class AddAddresseComponent implements OnInit, OnDestroy {
  @Input() id: string;
  private nativeElement: any;
  private conversation: Conversation;

  public params: any[] = [];

  constructor(private modalService: ModalService, private element: ElementRef) {
    this.nativeElement = element.nativeElement;
  }

  ngOnInit() {
      const modal = this;

      this.nativeElement.addEventListener('click', function (e: any) {
          if (e.target.className === 'app-modal') {
              modal.close();
          }
      });

      this.modalService.add(this);
  }

  ngOnDestroy(): void {
      this.modalService.remove(this.id);
      this.nativeElement.remove();
  }

  open(): void {
      this.nativeElement.style.display = 'block';
      document.body.classList.add('app-modal-open');

      if (this.params !== undefined) {
        const possibleConversations: any[] = this.params.filter(x => x instanceof Conversation);
        if (possibleConversations) {
            this.conversation = possibleConversations[0] as Conversation;
        }
      }
  }

  close(): void {
      this.nativeElement.style.display = 'none';
      document.body.classList.remove('app-modal-open');
  }
}
